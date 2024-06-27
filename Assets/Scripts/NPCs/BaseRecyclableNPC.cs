public class BaseRecyclableNPC : StateMachine<BaseRecyclableNPC>
{
    public virtual RecyclableType recyclableType {get;}
    public virtual bool is_food {get; }
    public virtual bool is_contaminated {get;}
}
