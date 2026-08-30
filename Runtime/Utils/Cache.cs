using System;
using System.Collections.Generic;
using System.Reflection;

namespace BIG
{
    /// <summary>
    /// Central registry of all static caches in the BIG ecosystem — one place to reset them all.
    /// Call <see cref="Clear"/> from the engine bootstrap (e.g. BIG.Unity on game start / editor play mode entry)
    /// so cached metadata never leaks between sessions when Unity domain reload is disabled.
    ///
    /// Pattern for cache owners:
    /// - expose a public static ClearCache() method AND register it here (see the static constructor for BIG's own caches;
    ///   downstream libraries call <see cref="Register"/> in their bootstrap/module),
    /// - generic caches that cannot be listed up-front (like EnumCache&lt;T&gt;) self-register lazily on first use —
    ///   if a cache was never touched, there is nothing to clear, so lazy registration stays correct.
    ///
    /// <see cref="ValidateCoverage"/> is the dev-time safety net: it scans loaded assemblies for ClearCache() methods
    /// that were never registered here — call it from editor tooling and log every finding.
    /// </summary>
    public static class Cache
    {
        private static readonly object LOCK = new object();
        private static readonly Dictionary<string, Action> CLEAR_ACTIONS = new Dictionary<string, Action>();

        static Cache()
        {
            // Caches owned by BIG itself. Downstream libraries register theirs via Register().
            CLEAR_ACTIONS[nameof(ReflectionExtension)] = ReflectionExtension.ClearCache;
            CLEAR_ACTIONS[nameof(RuntimeDependencyProvider)] = RuntimeDependencyProvider.ClearCache;
            CLEAR_ACTIONS[nameof(EventsUtils)] = EventsUtils.ClearCache;
        }

        /// <summary>
        /// Register a cache clearing action under a unique name (use the owning type name).
        /// Registering the same name again overwrites the previous action.
        /// </summary>
        public static void Register(string name, Action clearAction)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (clearAction == null) throw new ArgumentNullException(nameof(clearAction));

            lock (LOCK)
            {
                CLEAR_ACTIONS[name] = clearAction;
            }
        }

        /// <summary>
        /// Clear every registered cache. Safe to call at any time; caches repopulate lazily on next use.
        /// </summary>
        public static void Clear()
        {
            Action[] actions;
            lock (LOCK)
            {
                actions = new Action[CLEAR_ACTIONS.Count];
                CLEAR_ACTIONS.Values.CopyTo(actions, 0);
            }

            foreach (var action in actions)
            {
                action();
            }
        }

        /// <summary>
        /// Number of currently registered caches (diagnostics).
        /// </summary>
        public static int RegisteredCount
        {
            get
            {
                lock (LOCK)
                {
                    return CLEAR_ACTIONS.Count;
                }
            }
        }

        /// <summary>
        /// Dev-time safety net: find every public static parameterless ClearCache() method in loaded assemblies
        /// whose owning type is NOT registered here. Call from editor tooling and log the result —
        /// every returned name is a cache that <see cref="Clear"/> will silently miss.
        /// Only the BIG ecosystem is scanned: BIG itself and assemblies that reference BIG —
        /// foreign code (Unity packages, third-party plugins) may use the same method name for its own purposes.
        /// </summary>
        /// <returns>Full names of types exposing an unregistered ClearCache().</returns>
        public static List<string> ValidateCoverage()
        {
            var missing = new List<string>();

            HashSet<string> registered;
            lock (LOCK)
            {
                registered = new HashSet<string>(CLEAR_ACTIONS.Keys);
            }

            var bigAssembly = typeof(Cache).Assembly;
            var bigName = bigAssembly.GetName().Name;
            var relevantAssemblies = new Dictionary<Assembly, bool>();

            foreach (var type in ReflectionExtension.GetAllTypes())
            {
                if (!relevantAssemblies.TryGetValue(type.Assembly, out bool relevant))
                {
                    relevant = IsBigEcosystemAssembly(type.Assembly, bigAssembly, bigName);
                    relevantAssemblies[type.Assembly] = relevant;
                }

                if (!relevant)
                    continue;

                MethodInfo? method;
                try
                {
                    method = type.GetMethod("ClearCache", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null);
                }
                catch
                {
                    continue; // Ambiguous or unloadable member — not our convention, skip.
                }

                if (method == null || method.ReturnType != typeof(void))
                    continue;

                if (!registered.Contains(type.Name))
                {
                    missing.Add(type.FullName ?? type.Name);
                }
            }

            return missing;
        }

        /// <summary>
        /// The ClearCache convention applies only to code built on BIG: BIG itself and assemblies referencing it.
        /// </summary>
        private static bool IsBigEcosystemAssembly(Assembly assembly, Assembly bigAssembly, string? bigName)
        {
            if (assembly == bigAssembly)
                return true;

            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                if (reference.Name == bigName)
                    return true;
            }

            return false;
        }
    }
}
