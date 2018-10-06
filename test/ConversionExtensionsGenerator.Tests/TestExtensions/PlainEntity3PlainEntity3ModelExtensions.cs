/* This class is auto generated */
using ConversionExtensionsGenerator.Tests.DataModels;
using ConversionExtensionsGenerator.Tests.Models;
using System;

namespace ConversionExtensionsGenerator.Tests.TestExtensions
{
    public static partial class PlainEntity3PlainEntity3ModelExtensions
    {
        public static PlainEntity3 PlainEntity3ToPlainEntity3Model(this PlainEntity3Model plainEntity3Model)
        {
            if (plainEntity3Model == null)
            {
                return null;
            }

            PlainEntity3 plainEntity3 = new PlainEntity3();
            plainEntity3.GuidProperty = plainEntity3Model.GuidProperty;
            plainEntity3.BoolProperty = plainEntity3Model.BoolProperty;
            plainEntity3.LongProperty = plainEntity3Model.LongProperty;
            plainEntity3.StringProperty = plainEntity3Model.StringProperty;
            plainEntity3.GuidField = plainEntity3Model.GuidField;
            plainEntity3.BoolField = plainEntity3Model.BoolField;
            plainEntity3.LongField = plainEntity3Model.LongField;
            plainEntity3.StringField = plainEntity3Model.StringField;
            return plainEntity3;
        }

        public static PlainEntity3Model PlainEntity3ModelToPlainEntity3(this PlainEntity3 plainEntity3)
        {
            if (plainEntity3 == null)
            {
                return null;
            }

            PlainEntity3Model plainEntity3Model = new PlainEntity3Model();
            plainEntity3Model.GuidProperty = plainEntity3.GuidProperty;
            plainEntity3Model.BoolProperty = plainEntity3.BoolProperty;
            plainEntity3Model.LongProperty = plainEntity3.LongProperty;
            plainEntity3Model.StringProperty = plainEntity3.StringProperty;
            plainEntity3Model.GuidField = plainEntity3.GuidField;
            plainEntity3Model.BoolField = plainEntity3.BoolField;
            plainEntity3Model.LongField = plainEntity3.LongField;
            plainEntity3Model.StringField = plainEntity3.StringField;
            return plainEntity3Model;
        }
    }
}