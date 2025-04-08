#if BIG_WORKBOOK
// Copyright (c) 2025, Big Ice Games
// All rights reserved.

using System;

namespace BIG.Workbook
{
    /// <summary>
    /// For classes decorated by <see cref="ClassMappingAttribute"/> with Automapping == false.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FieldMappingAttribute : Attribute
    {
        public string WorkbookHeader { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workbookHeader">Header/Column name from the workbook.</param>
        public FieldMappingAttribute(string workbookHeader)
        {
            WorkbookHeader = workbookHeader;
        }
    }
}
#endif