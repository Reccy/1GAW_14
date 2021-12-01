using BehaviorDesigner.Runtime.Tasks;

public class IsOnFire : Conditional
{
    private Flammable m_flammable;

    public override void OnAwake()
    {
        m_flammable = GetComponent<Flammable>();
    }

    public override TaskStatus OnUpdate()
    {
        if (m_flammable.OnFire)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}
