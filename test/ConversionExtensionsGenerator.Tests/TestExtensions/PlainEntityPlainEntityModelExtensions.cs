/* This class is auto generated */
using ConversionExtensionsGenerator.Tests.DataModels;
using ConversionExtensionsGenerator.Tests.Models;
using System;

namespace ConversionExtensionsGenerator.Tests.TestExtensions
{
    public static partial class PlainEntityPlainEntityModelExtensions
    {
        public static PlainEntity PlainEntityToPlainEntityModel(this PlainEntityModel plainEntityModel)
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
            plainEntity.IntField = plainEntityModel.IntField;
            plainEntity.LongField = plainEntityModel.LongField;
            plainEntity.StringField = plainEntityModel.StringField;
            plainEntity.StringField2 = plainEntityModel.StringField2;
            return plainEntity;
        }

        public static PlainEntityModel PlainEntityModelToPlainEntity(this PlainEntity plainEntity)
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
            plainEntityModel.IntField = plainEntity.IntField;
            plainEntityModel.LongField = plainEntity.LongField;
            plainEntityModel.StringField = plainEntity.StringField;
            plainEntityModel.StringField2 = plainEntity.StringField2;
            return plainEntityModel;
        }
    }
}