namespace ConversionExtensionsGenerator.Tests.Models;

public partial class PlainEntityModel
{
    public int IntProperty { get; set; }

    public long LongProperty { get; set; }

    public string StringProperty { get; set; }

    public string String2Property { get; set; }

    public virtual PlainEntity2Model PlainEntityClass2Property { get; set; }

    public virtual ICollection<PlainEntity2Model> PlainEntitiesCollectionProperty { get; set; }

    public int IntField;

    public long LongField;

    public string StringField;

    public string StringField2;

    public PlainEntity2Model PlainEntityClass2Field;

    public ICollection<PlainEntity2Model> PlainEntitiesCollectionField;
}

public partial class PlainEntity2Model
{
    public int IntProperty { get; set; }

    public Guid GuidProperty { get; set; }

    public bool BoolProperty { get; set; }

    public long LongProperty { get; set; }

    public string StringProperty { get; set; }

    public string StringProperty2 { get; set; }

    public IEnumerable<PlainEntity3Model> PlainEntitiesCollectionProperty { get; set; }

    public int IntField;

    public Guid GuidField;

    public bool BoolField;

    public long LongField;

    public string StringField;

    public string StringField2;

    public IEnumerable<PlainEntity3Model> PlainEntitiesCollectionField;
}

public partial class PlainEntity3Model
{
    public Guid GuidProperty { get; set; }

    public bool BoolProperty { get; set; }

    public long LongProperty { get; set; }

    public string StringProperty { get; set; }

    public TestEnumModel TestEnumProperty { get; set; }

    public TestEnumModel TestEnum3Property { get; set; }

    public Guid GuidField;

    public bool BoolField;

    public long LongField;

    public string StringField;

    public TestEnumModel TestEnumField;

    public TestEnumModel TestEnum3Field;
}

public partial class EntitySomeModel
{
    public Guid GuidProperty { get; set; }

    public bool BoolProperty { get; set; }

    public long LongProperty { get; set; }

    public string StringProperty { get; set; }

    public TestEnumModel TestEnumProperty { get; set; }

    public TestEnumModel TestEnumProperty2 { get; set; }

    public Guid GuidField;

    public bool BoolField;

    public long LongField;

    public string StringField;

    public TestEnumModel TestEnumField;

    public TestEnumModel TestEnumField2;
}

public enum TestEnumModel
{
    One,
    Two,
    Three,
    Four
}