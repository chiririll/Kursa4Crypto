namespace Kursa4Crypto.Protocol;

public class IdService
{
    private static int nextProverId = 1;
    private static int nextVerifierId = 1;

    public static int GetProverId() => nextProverId++;
    public static int GetVerifierId() => nextVerifierId++;
}
