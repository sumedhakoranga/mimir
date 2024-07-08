using System.Security.Cryptography;
using Bencodex;
using Bencodex.Types;
using Lib9c;
using Libplanet.Common;
using Mimir.Models.Stat;
using Nekoyume.Model.State;

namespace Mimir.Models.Item;

public class Equipment : ItemUsable, IBencodable
{
    public bool Equipped { get; private set; }
    public int Level { get; private set; }
    public long? Exp { get; private set; }
    public int? OptionCountFromCombination { get; private set; }

    public DecimalStat Stat { get; }
    public int SetId { get; private set; }
    public string SpineResourcePath { get; private set; }
    public bool MadeWithMimisbrunnrRecipe { get; private set; }

    public Equipment(Dictionary bencoded)
        : base(bencoded)
    {
        Equipped = bencoded["equipped"].ToBoolean();
        Level = bencoded["level"].ToInteger();
        Stat = new DecimalStat((Dictionary)bencoded["stat"]);
        SetId = bencoded["set_id"].ToInteger();
        SpineResourcePath = bencoded["spine_resource_path"].ToDotnetString();
        OptionCountFromCombination = bencoded["oc"]?.ToInteger();

        if (bencoded.TryGetValue((Text)SerializeKeys.MadeWithMimisbrunnrRecipeKey, out var value))
        {
            MadeWithMimisbrunnrRecipe = value.ToBoolean();
        }

        if (bencoded.TryGetValue((Text)SerializeKeys.EquipmentExpKey, out value))
        {
            try
            {
                Exp = value.ToLong();
            }
            catch (InvalidCastException)
            {
                Exp = (long)((Integer)value).Value;
            }
        }
        else
        {
            Exp = 0L;
        }
    }

    new public IValue Bencoded => Serialize();

    public IValue Serialize()
    {
        var dict = ((Dictionary)base.Bencoded)
            .Add("equipped", Equipped.Serialize())
            .Add("level", Level.Serialize())
            .Add("stat", Stat.Bencoded)
            .Add("set_id", SetId.Serialize())
            .Add("spine_resource_path", SpineResourcePath.Serialize());

        if (OptionCountFromCombination > 0)
        {
            dict = dict.SetItem("oc", OptionCountFromCombination.Serialize());
        }

        if (MadeWithMimisbrunnrRecipe)
        {
            dict = dict.SetItem("mwmr", MadeWithMimisbrunnrRecipe.Serialize());
        }

        if (Exp > 0)
        {
            dict = dict.SetItem("eq_exp", Exp.Serialize());
        }

        return dict;
    }
}