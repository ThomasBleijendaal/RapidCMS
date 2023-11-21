using System;
using System.Collections.Generic;
using RapidCMS.Core.Attributes;

namespace RapidCMS.Core.Enums;

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
    /// A dropdown accepting a string, requires a data provider to provide the options.
    /// 
    /// NOTE: Consider using EntityPicker. That control has better UX but is somewhat bigger.
    /// 
    /// NOTE: This control will reset its value when the DataCollection data changes and the selected item is not available anymore.
    /// </summary>
    [Relation(RelationType.One)]
    Dropdown,

    /// <summary>
    /// A list of options accepting a string, requires a data provider to provide the options.
    /// 
    /// NOTE: Consider using EntityPicker. That control has better UX but is somewhat bigger.
    ///  
    /// NOTE: This control will reset its value when the DataCollection data changes and the selected item is not available anymore.
    /// </summary>
    [Relation(RelationType.One)]
    Select,

    /// <summary>
    /// A list of options accepting an array of strings, requires a data provider to provide the options, returning the selected items via the RelationContainer in EditContext
    /// 
    /// NOTE: Consider using EntitiesPicker. That control has better UX but is somewhat bigger. 
    /// 
    /// NOTE: This control will reset its value when the DataCollection data changes and any of the selected items are not available anymore.
    /// </summary>
    [Relation(RelationType.Many)]
    MultiSelect,

    /// <summary>
    /// A list editor accepting an array of strings
    /// </summary>
    [DefaultType(typeof(IEnumerable<string>), typeof(ICollection<string>), typeof(IList<string>), typeof(List<string>))]
    ListEditor,

    /// <summary>
    /// A picker with search and navigation features for selecting an entity
    /// 
    /// NOTE: This control will not reset its value when the DataCollection data changes.
    /// </summary>
    [Relation(RelationType.One)]
    EntityPicker,

    /// <summary>
    /// A picker with search and navigation features for selecting multiple entities
    /// 
    /// NOTE: This control will not reset its value when the DataCollection data changes.
    /// </summary>
    [Relation(RelationType.Many)]
    EntitiesPicker,

    /// <summary>
    /// An editor that will render simple editors for each of the properties of the model it edits.
    /// 
    /// Only creates basic editors for properties (TextBox, TextArea, Numeric, Checkbox, Date), use [DisplayAttribute] for customizations.
    /// 
    /// Model property must be annotated with [ValidateObject] to enable validation.
    /// </summary>
    ModelEditor,

    /// <summary>
    /// An editor that will allow the user to select or deselect flags of an enum. The value will be saved as the composite value of the enum.
    /// 
    /// Requires to have the EnumDataProvider DataCollection attached and the enum must be an Int32 enum.
    /// </summary>
    [Relation(RelationType.One)]
    EnumFlagPicker
}
