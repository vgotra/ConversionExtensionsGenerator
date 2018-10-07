/* This class is auto generated */
using System;
using System.Collections.Generic;
using System.Linq;
using ConversionExtensionsGenerator.Tests.DataModels;
using ConversionExtensionsGenerator.Tests.Models;
using ConversionExtensionsGenerator.Tests.TestExtensions;

namespace ConversionExtensionsGenerator.Tests.TestExtensions
{
    public static partial class PlainEntity2PlainEntity2ModelExtensions
    {
        public static PlainEntity2 PlainEntity2ModelToPlainEntity2(this PlainEntity2Model plainEntity2Model)
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
            plainEntity2.PlainEntitiesCollectionProperty = plainEntity2Model.PlainEntitiesCollectionProperty.PlainEntity3ModelCollectionToPlainEntity3Collection();
            plainEntity2.IntField = plainEntity2Model.IntField;
            plainEntity2.GuidField = plainEntity2Model.GuidField;
            plainEntity2.BoolField = plainEntity2Model.BoolField;
            plainEntity2.LongField = plainEntity2Model.LongField;
            plainEntity2.StringField = plainEntity2Model.StringField;
            plainEntity2.StringField2 = plainEntity2Model.StringField2;
            plainEntity2.PlainEntitiesCollectionField = plainEntity2Model.PlainEntitiesCollectionField.PlainEntity3ModelCollectionToPlainEntity3Collection();
            return plainEntity2;
        }

        public static List<PlainEntity2> PlainEntity2ModelCollectionToPlainEntity2Collection(this IEnumerable<PlainEntity2Model> plainEntity2ModelCollection)
        {
            if (plainEntity2ModelCollection == null)
            {
                return null;
            }

            List<PlainEntity2> plainEntity2Collection = plainEntity2ModelCollection.Select(x => x.PlainEntity2ModelToPlainEntity2()).ToList();
            return plainEntity2Collection;
        }

        public static PlainEntity2Model PlainEntity2ToPlainEntity2Model(this PlainEntity2 plainEntity2)
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
            plainEntity2Model.PlainEntitiesCollectionProperty = plainEntity2.PlainEntitiesCollectionProperty.PlainEntity3CollectionToPlainEntity3ModelCollection();
            plainEntity2Model.IntField = plainEntity2.IntField;
            plainEntity2Model.GuidField = plainEntity2.GuidField;
            plainEntity2Model.BoolField = plainEntity2.BoolField;
            plainEntity2Model.LongField = plainEntity2.LongField;
            plainEntity2Model.StringField = plainEntity2.StringField;
            plainEntity2Model.StringField2 = plainEntity2.StringField2;
            plainEntity2Model.PlainEntitiesCollectionField = plainEntity2.PlainEntitiesCollectionField.PlainEntity3CollectionToPlainEntity3ModelCollection();
            return plainEntity2Model;
        }

        public static List<PlainEntity2Model> PlainEntity2CollectionToPlainEntity2ModelCollection(this IEnumerable<PlainEntity2> plainEntity2Collection)
        {
            if (plainEntity2Collection == null)
            {
                return null;
            }

            List<PlainEntity2Model> plainEntity2ModelCollection = plainEntity2Collection.Select(x => x.PlainEntity2ToPlainEntity2Model()).ToList();
            return plainEntity2ModelCollection;
        }
    }
}