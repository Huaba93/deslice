namespace SIMS_Backend;

public interface ISIMSContextFactory
{
    public abstract SIMSContext Create();
}

public class SIMSContextFactory : ISIMSContextFactory
{
    public SIMSContext Create()
    {
        return new SIMSContext();
    }
}