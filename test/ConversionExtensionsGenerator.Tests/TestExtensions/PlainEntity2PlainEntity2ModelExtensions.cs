/* This class is auto generated */
using ConversionExtensionsGenerator.Tests.DataModels;
using ConversionExtensionsGenerator.Tests.Models;
using System;

namespace ConversionExtensionsGenerator.Tests.TestExtensions
{
    public static partial class PlainEntity2PlainEntity2ModelExtensions
    {
        public static PlainEntity2 PlainEntity2ToPlainEntity2Model(this PlainEntity2Model plainEntity2Model)
        {
            if (plainEntity2Model == null)
            {
                return null;
            }

            PlainEntity2 plainEntity2 = new PlainEntity2();
            plainEntity2.IntProperty = plainEntity2Model.IntProperty;
            plainEntity2.GuidProperty = plainEntity2Model.GuidProperty;
            plainEntity2.BoolProperty = plainEntity2Model.BoolProperty;
            plainEntity2.LongProperty = plainEntity2Model.LongProperty;
            plainEntity2.StringProperty = plainEntity2Model.StringProperty;
            plainEntity2.StringProperty2 = plainEntity2Model.StringProperty2;
            plainEntity2.IntField = plainEntity2Model.IntField;
            plainEntity2.GuidField = plainEntity2Model.GuidField;
            plainEntity2.BoolField = plainEntity2Model.BoolField;
            plainEntity2.LongField = plainEntity2Model.LongField;
            plainEntity2.StringField = plainEntity2Model.StringField;
            plainEntity2.StringField2 = plainEntity2Model.StringField2;
            return plainEntity2;
        }

        public static PlainEntity2Model PlainEntity2ModelToPlainEntity2(this PlainEntity2 plainEntity2)
        {
            if (plainEntity2 == null)
            {
                return null;
            }

            PlainEntity2Model plainEntity2Model = new PlainEntity2Model();
            plainEntity2Model.IntProperty = plainEntity2.IntProperty;
            plainEntity2Model.GuidProperty = plainEntity2.GuidProperty;
            plainEntity2Model.BoolProperty = plainEntity2.BoolProperty;
            plainEntity2Model.LongProperty = plainEntity2.LongProperty;
            plainEntity2Model.StringProperty = plainEntity2.StringProperty;
            plainEntity2Model.StringProperty2 = plainEntity2.StringProperty2;
            plainEntity2Model.IntField = plainEntity2.IntField;
            plainEntity2Model.GuidField = plainEntity2.GuidField;
            plainEntity2Model.BoolField = plainEntity2.BoolField;
            plainEntity2Model.LongField = plainEntity2.LongField;
            plainEntity2Model.StringField = plainEntity2.StringField;
            plainEntity2Model.StringField2 = plainEntity2.StringField2;
            return plainEntity2Model;
        }
    }
}