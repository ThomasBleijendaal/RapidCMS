using System;
using System.Collections.Generic;
using RapidCMS.Core.Attributes;

namespace RapidCMS.Core.Enums
{
    public enum EditorType
    {
        None = -99,
        Custom = -1,

        /// <summary>
        /// A simple textbox accepting a string
        /// </summary>
        TextBox = 0,

        /// <summary>
        /// A simple textarea accepting a string
        /// </summary>
        TextArea,

        /// <summary>
        /// A disabled textbox accepting a string
        /// </summary>
        Readonly,

        /// <summary>
        /// A simple textbox accepting a number (with or without decimals), limiting the input to numbers only
        /// </summary>
        [DefaultType(typeof(int), typeof(long), typeof(uint), typeof(ulong), typeof(int?), typeof(long?), typeof(uint?), typeof(ulong?))]
        Numeric,

        /// <summary>
        /// A simple checkbox accepting a boolean
        /// </summary>
        [DefaultType(typeof(bool), typeof(bool?))]
        Checkbox,

        /// <summary>
        /// A simple textbox accepting a date, limiting the input to dates only
        /// </summary>
        [DefaultType(typeof(DateTime))]
        Date,

        /// <summary>
        /// A dropdown accepting a string, requires a data provider to provide the options
        /// </summary>
        [Relation(RelationType.One)]
        Dropdown,

        /// <summary>
        /// A list of options accepting a string, requires a data provider to privide the options
        /// </summary>
        [Relation(RelationType.One)]
        Select,

        /// <summary>
        /// A list of options acceting an array of strings, requires a data provider to provide the options, returning the selected items via the RelationContainer in EditContext
        /// </summary>
        [Relation(RelationType.Many)]
        MultiSelect,

        /// <summary>
        /// A list editor accepting an array of strings
        /// </summary>
        [DefaultType(typeof(IEnumerable<string>), typeof(ICollection<string>), typeof(IList<string>), typeof(List<string>))]
        ListEditor
    }
}
