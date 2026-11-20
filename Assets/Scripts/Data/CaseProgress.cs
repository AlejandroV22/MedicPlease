using System;
using System.Linq;

public static class CaseProgress
{
    // Orden de desbloqueo. Los nombres deben coincidir con clinicalCase.name y con caseId.
    public static readonly string[] CaseOrder = { "Tutorial", "Situacion1", "Situacion2" };

    public static bool IsCaseUnlocked(GameData data, string caseName)
    {
        int index = Array.IndexOf(CaseOrder, caseName);
        if (index < 0) return false;   // Caso no registrado en el orden
        if (index == 0) return true;   // El tutorial siempre está disponible
        if (data == null) return false;

        string previous = CaseOrder[index - 1];
        CaseRecord previousRecord = data.caseHistory.FirstOrDefault(c => c.caseId == previous);

        return previousRecord != null && previousRecord.DiagnosedCorrectly;
    }
}