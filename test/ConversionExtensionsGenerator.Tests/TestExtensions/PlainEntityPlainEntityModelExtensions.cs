/* This class is auto generated */
using System;
using System.Collections.Generic;
using System.Linq;
using ConversionExtensionsGenerator.Tests.DataModels;
using ConversionExtensionsGenerator.Tests.Models;
using ConversionExtensionsGenerator.Tests.TestExtensions;

namespace ConversionExtensionsGenerator.Tests.TestExtensions
{
    public static partial class PlainEntityPlainEntityModelExtensions
    {
        public static PlainEntity PlainEntityModelToPlainEntity(this PlainEntityModel plainEntityModel)
        {
            if (plainEntityModel == null)
            {
                return null;
            }

            PlainEntity plainEntity = new PlainEntity();
            plainEntity.IntProperty = plainEntityModel.IntProperty;
            plainEntity.LongProperty = plainEntityModel.LongProperty;
            plainEntity.StringProperty = plainEntityModel.StringProperty;
            plainEntity.String2Property = plainEntityModel.String2Property;
            plainEntity.PlainEntityClass2Property = plainEntityModel.PlainEntityClass2Property.PlainEntity2ModelToPlainEntity2();
            plainEntity.PlainEntitiesCollectionProperty = plainEntityModel.PlainEntitiesCollectionProperty.PlainEntity2ModelCollectionToPlainEntity2Collection();
            plainEntity.IntField = plainEntityModel.IntField;
            plainEntity.LongField = plainEntityModel.LongField;
            plainEntity.StringField = plainEntityModel.StringField;
            plainEntity.StringField2 = plainEntityModel.StringField2;
            plainEntity.PlainEntityClass2Field = plainEntityModel.PlainEntityClass2Field.PlainEntity2ModelToPlainEntity2();
            plainEntity.PlainEntitiesCollectionField = plainEntityModel.PlainEntitiesCollectionField.PlainEntity2ModelCollectionToPlainEntity2Collection();
            return plainEntity;
        }

        public static List<PlainEntity> PlainEntityModelCollectionToPlainEntityCollection(this IEnumerable<PlainEntityModel> plainEntityModelCollection)
        {
            if (plainEntityModelCollection == null)
            {
                return null;
            }

            List<PlainEntity> plainEntityCollection = plainEntityModelCollection.Select(x => x.PlainEntityModelToPlainEntity()).ToList();
            return plainEntityCollection;
        }

        public static PlainEntityModel PlainEntityToPlainEntityModel(this PlainEntity plainEntity)
        {
            if (plainEntity == null)
            {
                return null;
            }

            PlainEntityModel plainEntityModel = new PlainEntityModel();
            plainEntityModel.IntProperty = plainEntity.IntProperty;
            plainEntityModel.LongProperty = plainEntity.LongProperty;
            plainEntityModel.StringProperty = plainEntity.StringProperty;
            plainEntityModel.String2Property = plainEntity.String2Property;
            plainEntityModel.PlainEntityClass2Property = plainEntity.PlainEntityClass2Property.PlainEntity2ToPlainEntity2Model();
            plainEntityModel.PlainEntitiesCollectionProperty = plainEntity.PlainEntitiesCollectionProperty.PlainEntity2CollectionToPlainEntity2ModelCollection();
            plainEntityModel.IntField = plainEntity.IntField;
            plainEntityModel.LongField = plainEntity.LongField;
            plainEntityModel.StringField = plainEntity.StringField;
            plainEntityModel.StringField2 = plainEntity.StringField2;
            plainEntityModel.PlainEntityClass2Field = plainEntity.PlainEntityClass2Field.PlainEntity2ToPlainEntity2Model();
            plainEntityModel.PlainEntitiesCollectionField = plainEntity.PlainEntitiesCollectionField.PlainEntity2CollectionToPlainEntity2ModelCollection();
            return plainEntityModel;
        }

        public static List<PlainEntityModel> PlainEntityCollectionToPlainEntityModelCollection(this IEnumerable<PlainEntity> plainEntityCollection)
        {
            if (plainEntityCollection == null)
            {
                return null;
            }

            List<PlainEntityModel> plainEntityModelCollection = plainEntityCollection.Select(x => x.PlainEntityToPlainEntityModel()).ToList();
            return plainEntityModelCollection;
        }
    }
}