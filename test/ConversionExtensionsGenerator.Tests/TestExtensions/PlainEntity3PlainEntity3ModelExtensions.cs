/* This class is auto generated */
using System;
using System.Collections.Generic;
using System.Linq;
using ConversionExtensionsGenerator.Tests.DataModels;
using ConversionExtensionsGenerator.Tests.Models;
using ConversionExtensionsGenerator.Tests.TestExtensions;

namespace ConversionExtensionsGenerator.Tests.TestExtensions
{
    public static partial class PlainEntity3PlainEntity3ModelExtensions
    {
        public static PlainEntity3 PlainEntity3ModelToPlainEntity3(this PlainEntity3Model plainEntity3Model)
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
            plainEntity3.TestEnumProperty = (TestEnum)plainEntity3Model.TestEnumProperty;
            plainEntity3.TestEnum3Property = (TestEnum)plainEntity3Model.TestEnum3Property;
            plainEntity3.GuidField = plainEntity3Model.GuidField;
            plainEntity3.BoolField = plainEntity3Model.BoolField;
            plainEntity3.LongField = plainEntity3Model.LongField;
            plainEntity3.StringField = plainEntity3Model.StringField;
            plainEntity3.TestEnumField = (TestEnum)plainEntity3Model.TestEnumField;
            plainEntity3.TestEnum3Field = (TestEnum)plainEntity3Model.TestEnum3Field;
            return plainEntity3;
        }

        public static List<PlainEntity3> PlainEntity3ModelCollectionToPlainEntity3Collection(this IEnumerable<PlainEntity3Model> plainEntity3ModelCollection)
        {
            if (plainEntity3ModelCollection == null)
            {
                return null;
            }

            List<PlainEntity3> plainEntity3Collection = plainEntity3ModelCollection.Select(x => x.PlainEntity3ModelToPlainEntity3()).ToList();
            return plainEntity3Collection;
        }

        public static PlainEntity3Model PlainEntity3ToPlainEntity3Model(this PlainEntity3 plainEntity3)
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
            plainEntity3Model.TestEnumProperty = (TestEnumModel)plainEntity3.TestEnumProperty;
            plainEntity3Model.TestEnum3Property = (TestEnumModel)plainEntity3.TestEnum3Property;
            plainEntity3Model.GuidField = plainEntity3.GuidField;
            plainEntity3Model.BoolField = plainEntity3.BoolField;
            plainEntity3Model.LongField = plainEntity3.LongField;
            plainEntity3Model.StringField = plainEntity3.StringField;
            plainEntity3Model.TestEnumField = (TestEnumModel)plainEntity3.TestEnumField;
            plainEntity3Model.TestEnum3Field = (TestEnumModel)plainEntity3.TestEnum3Field;
            return plainEntity3Model;
        }

        public static List<PlainEntity3Model> PlainEntity3CollectionToPlainEntity3ModelCollection(this IEnumerable<PlainEntity3> plainEntity3Collection)
        {
            if (plainEntity3Collection == null)
            {
                return null;
            }

            List<PlainEntity3Model> plainEntity3ModelCollection = plainEntity3Collection.Select(x => x.PlainEntity3ToPlainEntity3Model()).ToList();
            return plainEntity3ModelCollection;
        }
    }
}