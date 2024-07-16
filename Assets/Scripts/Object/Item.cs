public class Item : InteractionObject
{

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
