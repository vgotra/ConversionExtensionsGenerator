using System;
using System.Collections.Generic;

namespace ConversionExtensionsGenerator.Tests.DataModels
{
    //TODO: Add cases with only get or set, ctor, etc.
    public partial class PlainEntity
    {
        public int IntProperty { get; set; }

        public long LongProperty { get; set; }

        public string StringProperty { get; set; }

        public string String2Property { get; set; }

        public virtual PlainEntity2 PlainEntityClass2Property { get; set; }

        public virtual ICollection<PlainEntity2> PlainEntitiesCollectionProperty { get; set; }

        public int IntField;

        public long LongField;

        public string StringField;

        public string StringField2;

        public PlainEntity2 PlainEntityClass2Field;

        public ICollection<PlainEntity2> PlainEntitiesCollectionField;
    }

    public partial class PlainEntity2
    {
        public int IntProperty { get; set; }

        public Guid GuidProperty { get; set; }

        public bool BoolProperty { get; set; }

        public long LongProperty { get; set; }

        public string StringProperty { get; set; }

        public string StringProperty2 { get; set; }

        public IEnumerable<PlainEntity3> PlainEntitiesCollectionProperty { get; set; }

        public int IntField;

        public Guid GuidField;

        public bool BoolField;

        public long LongField;

        public string StringField;

        public string StringField2;

        public IEnumerable<PlainEntity3> PlainEntitiesCollectionField;
    }

    public partial class PlainEntity3
    {
        public Guid GuidProperty { get; set; }

        public bool BoolProperty { get; set; }

        public long LongProperty { get; set; }

        public string StringProperty { get; set; }

        public TestEnum TestEnumProperty { get; set; }

        public TestEnum TestEnum3Property { get; set; }

        public Guid GuidField;

        public bool BoolField;

        public long LongField;

        public string StringField;

        public TestEnum TestEnumField;

        public TestEnum TestEnum3Field;
    }

    public partial class SomeEntity
    {
        public Guid GuidProperty { get; set; }

        public bool BoolProperty { get; set; }

        public long LongProperty { get; set; }

        public string StringProperty { get; set; }

        public TestEnum TestEnumProperty { get; set; }

        public TestEnum TestEnumProperty2 { get; set; }

        public Guid GuidField;

        public bool BoolField;

        public long LongField;

        public string StringField;

        public TestEnum TestEnumField;

        public TestEnum TestEnumField2;
    }

    public enum TestEnum
    {
        One,
        Two,
        Three
    }
}