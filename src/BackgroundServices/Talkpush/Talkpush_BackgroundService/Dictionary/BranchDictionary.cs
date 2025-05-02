namespace Talkpush_BackgroundService.Dictionary;

public class BranchDictionary
{
    private static readonly Dictionary<string, string> _branchDictionary = new Dictionary<string, string>
        {
            { "PH-QUE-BLDGL-Y0", "Concentrix UPA (UP QC)" },
            { "PH-CEB-ASIAT-Y1", "Concentrix Cebu" },
            { "PH-CEB-ASIAT-Y3", "Concentrix Cebu" },
            { "PH-CEB-MONTA-01", "Concentrix Cebu" },
            { "PH-CEB-BLDI3-Y0", "Concentrix Cebu" },
            { "PH-CEB-ASIAT-Y0", "Concentrix Cebu" },
            { "PH-QUE-UPDIL-Y0", "Concentrix UPA (UP QC)" },
            { "PH-QUE-UPAYA-08", "Concentrix UPA (UP QC)" },
            { "PH-QUE-UPAYA-06", "Concentrix UPA (UP QC)" },
            { "PH-QUE-TERAT-05", "Concentrix Bridgetowne" },
            { "PH-QUE-GIGAT-01", "Concentrix Bridgetowne" },
            { "PH-QUE-EXXAT-06", "Concentrix Bridgetowne" },
            { "PH-QUE-EXXAT-07", "Concentrix Bridgetowne" },
            { "PH-QUE-EXXAT-08", "Concentrix Bridgetowne" },
            { "PH-QUE-TERAT-10", "Concentrix Bridgetowne" },
            { "PPH-QUE-EXXAT-02", "Concentrix Bridgetowne" },
            { "PH-VELA-EXXA-1", "Concentrix Bridgetowne" },
            { "PH-QUE-EXXAT-05", "Concentrix Bridgetowne" },
            { "PH-BAC-SANPA-Y0", "Concentrix Bacolod" },
            { "PH-BAC-LACSO-Y0", "Concentrix Bacolod" },
            { "PH-DAV-ABREE-Y0", "Concentrix Davao-Abreeza" },
            { "PH-DAV-DAMOS-02", "Concentrix Davao-Abreeza" },
            { "PH-DAV-DAMOS-01", "Concentrix Davao-Abreeza" },
            { "PH-DAV-ABREE-Y1", "Concentrix Davao-Abreeza" },
            { "PH-MUN-CYBER-Y1", "Concentrix Alabang" },
            { "PH-MUN-CYBER-Y0", "Concentrix Alabang" },
            { "PH-MAK-BARAN-Y1", "Concentrix ANE (Ayala North Exchange) Makati" },
            { "PH-MAN-MEGAC-Y0", "Concentrix Megamall" },
            { "PH-CAG-TRADE-00", "Concentrix Cagayan de Oro" },
            { "PH-MAK-RUFIN-Y0", "Concentrix Makati" },
            { "PH-CUB-SPARK-06", "Concentrix Cubao" },
            { "PH-STA-YUSEC-Y0", "Concentrix San Lazaro" },
            { "PH-SAN-ESERV-Y0", "Concentrix Nuvali" },
            { "PH-CEB-CEBEX-01", "Concentrix Cebu Exchange" },
            { "PH-BAC-LACS0", "Concentrix Alabang"}
        };

    public static string GetBranchName(string branchCode)
    {
        if (_branchDictionary.TryGetValue(branchCode, out var branchName))
        {
            return branchName;
        }
        return "Unknown Branch";
    }
}
