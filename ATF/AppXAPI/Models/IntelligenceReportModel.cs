using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppXAPI.Models
{
    public class IntelligenceReportModel
    {
        public string Title { get; set; } = "Test" ;
        public string DateOfIntelligenceCollection { get; set; } = "01/01/2000";
        public IntelligenceReportSupportValuesModel IntelligenceReportSupportValuesModel { get; set; } = new IntelligenceReportSupportValuesModel();
        public string HeaderConfidenceLevel { get; set; } = "Medium";
        public ProtectiveMarking ProtectiveMarking { get; set; } = new ProtectiveMarking();
        public HandlingCode HandlingCode { get; set; } = new HandlingCode();
        public SensitiveSouce SensitiveSouce { get; set; } = new SensitiveSouce();
        public PrimarySource PrimarySource { get; set; } = new PrimarySource();
        public int NumberOfOtherSources { get; set; } = 0;
        public List<SecondarySource> SecondarySource { get; set; } = new List<SecondarySource> {  };
        public RiskAssessment RiskAssessment { get; set; } = new RiskAssessment();
        public string Case { get; set; } = "Titan";
    }

    public class RiskAssessment
    {
        public string CreateRiskAssessment { get; set; } = "No";
        public RiskAssessmentYes RiskAssessmentYes { get; set; } = new RiskAssessmentYes();
    }

    public class RiskAssessmentYes
    {
        public string? SensitiveMaterial { get; set; } = "No";
        public SensitiveMaterialYes SensitiveMaterialYes { get; set; } = new SensitiveMaterialYes();
        public string? Receipt { get; set; } = "PAPER";
        public string? Dissemination { get; set; } = "INCLUDES DISCLOSURE";
        public string? Use { get; set; }
        public string? PurposeDissemination { get; set; }
        public string? SpecialConditionsForRetention { get; set; } = "No";
        public string? SpecialConditionsForRetentionDetails { get; set; } = "HEllo";
        public string? RiskManagementPlan { get; set; } = "A PLAN";
        public string? LevelOfRisk { get; set; } = "Low";
        public string? ReviewDate { get; set; } = "TODAY+10";
        public ProtectiveMarking RiskProtectiveMarking { get; set; } = new ProtectiveMarking();

    }

    public class SensitiveMaterialYes
    {
        public string? CategoryOfMaterial { get; set; } = "NONE";
        public string? Restrictions { get; set; } = "UNKNOWN";
    }


    public class SecondarySource
    {
        public string? Source { get; set; } = "SOURCE";
        public string? IntelligenceDetails { get; set; } = "UNKNOWN";
        public string? SourceEvaluation { get; set; } = "1 - Reliable";
        public string? IntelligenceEvaluation { get; set; } = "A - Known directly";
    }

    public class PrimarySource
    {
        public string IntelligenceDetails { get; set; } = "UNKNOWN";
        public string SourceEvaluation { get; set; } = "1 - Reliable";
        public string IntelligenceEvaluation { get; set; } = "A - Known directly";
    }

    public class SensitiveSouce
    {
        public string Subtype { get; set; } = "Person";
        public string SourceReference { get; set; } = "X";
    }

    public class HandlingCode
    {
        public string Code { get; set; } = "P";
        public LawfulSharingPermitted LawfulSharingPermitted { get; set; } = new LawfulSharingPermitted();
    }

    public class LawfulSharingPermitted
    {
        public string? DetailedHandlingInstructions { get; set; } = null;
        public string? ActionCode { get; set; } = "A2";
        public string? SanitisationCode { get; set; } = "S2";
    }

    public class ProtectiveMarking
    {
        public string? Level { get; set; } = "OFFICIAL-SENSTIVE";
        public string? ReasonForChange { get; set; }
    }

    public class IntelligenceReportSupportValuesModel
    {
        public List<string> ThreatValues { get; set; } = new List<string> { };
        public List<string> HOCCValues { get; set; } = new List<string> { };
        public List<string> POAValues { get; set; } = new List<string> { };
    }

    public class GetIntelligenceReport
    {
        public static IntelligenceReportModel CreateIntelligenceReport()
        {
            IntelligenceReportModel returnIntelligenceReport = new IntelligenceReportModel();
            returnIntelligenceReport.Title = "X";


            return returnIntelligenceReport;
        }

        private static SecondarySource ReturnSecondarySource(string irName)
        {
            SecondarySource secondarySource = new SecondarySource();
            switch (irName.ToLower())
            {
                default:
                    {
                        secondarySource.Source = "The SOURCE";
                        secondarySource.IntelligenceDetails = "intel here";
                        secondarySource.SourceEvaluation = "2 - Untested";
                        secondarySource.IntelligenceEvaluation = "A - Known directly";
                        break;
                    }
                case "test":
                    {
                        secondarySource.Source = "The SOURCE 2";
                        secondarySource.IntelligenceDetails = "intel here";
                        secondarySource.SourceEvaluation = "2 - Untested";
                        secondarySource.IntelligenceEvaluation = "A - Known directly";
                        break;
                    }
            }
            return secondarySource;
        }

        public static IntelligenceReportModel GetIntelligenceReportModel(string irName)
        {
            IntelligenceReportModel returnIntelligenceReport = new IntelligenceReportModel();
            switch (irName.ToLower())
            {
                default:
                    {
                        returnIntelligenceReport.Title = irName;
                        returnIntelligenceReport.DateOfIntelligenceCollection = "TODAY";
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.ThreatValues.Add("Cross-Cutting|Prisons and lifetime management");
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.ThreatValues.Add("Cyber|Emerging new crime wave - Band 2");
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.HOCCValues.Clear();
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.POAValues.Clear();
                        returnIntelligenceReport.HeaderConfidenceLevel = "confidence level high";
                        returnIntelligenceReport.ProtectiveMarking.Level = "protective marking official-sensitive";
                        returnIntelligenceReport.ProtectiveMarking.ReasonForChange = null;
                        returnIntelligenceReport.HandlingCode.Code = "P";
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.DetailedHandlingInstructions = null;
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.ActionCode = "A3";
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.SanitisationCode = "S2";
                        returnIntelligenceReport.SensitiveSouce.Subtype = "Person";
                        returnIntelligenceReport.SensitiveSouce.SourceReference = "Reference of the source";
                        returnIntelligenceReport.PrimarySource.IntelligenceDetails = "Primary Source Details";
                        returnIntelligenceReport.PrimarySource.SourceEvaluation = "source evaluation 1 - reliable";
                        returnIntelligenceReport.PrimarySource.IntelligenceEvaluation = "A - Known directly";
                        returnIntelligenceReport.NumberOfOtherSources = 0;
                        returnIntelligenceReport.SecondarySource.Clear();
                        for (int i = 0; i < returnIntelligenceReport.NumberOfOtherSources; i++)
                        {
                            returnIntelligenceReport.SecondarySource.Add(ReturnSecondarySource("Secondary Source" + irName + i));
                        }
                        returnIntelligenceReport.RiskAssessment.CreateRiskAssessment = "risk assessment yes";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterial = "confidential material yes";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterialYes.CategoryOfMaterial = "Funny material";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterialYes.Restrictions = "Do not look at the light!";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Receipt = "REC123";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Dissemination = "I DISS this!";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Use = "It can be?";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.PurposeDissemination = "What can I want it for";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SpecialConditionsForRetention = "retention yes";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SpecialConditionsForRetentionDetails = "These conditions";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskManagementPlan = "There is a risk, and there is a plan!";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.LevelOfRisk = "Low";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.ReviewDate = "TODAY+20";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskProtectiveMarking.Level = "protective marking offical-sensitive";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskProtectiveMarking.ReasonForChange = null;
                        break;
                    }
                case "ir123457":
                    {
                        returnIntelligenceReport.Title = "Testing IR IR123457";
                        returnIntelligenceReport.DateOfIntelligenceCollection = "TODAY";
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.ThreatValues.Add("Cross-Cutting|Prisons and lifetime management");
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.ThreatValues.Add("Cyber|Emerging new crime wave - Band 2");
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.HOCCValues.Clear();
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.POAValues.Clear();
                        returnIntelligenceReport.HeaderConfidenceLevel = "confidence level low";
                        returnIntelligenceReport.ProtectiveMarking.Level = "protective marking official-sensitive";
                        returnIntelligenceReport.ProtectiveMarking.ReasonForChange = null;
                        returnIntelligenceReport.HandlingCode.Code = "C";
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.DetailedHandlingInstructions = "handling inst";
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.ActionCode = "A3";
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.SanitisationCode = "S2";
                        returnIntelligenceReport.SensitiveSouce.Subtype = "Person";
                        returnIntelligenceReport.SensitiveSouce.SourceReference = "Reference of the source";
                        returnIntelligenceReport.PrimarySource.IntelligenceDetails = "Primary Source Details";
                        returnIntelligenceReport.PrimarySource.SourceEvaluation = "source evaluation 1 - reliable";
                        returnIntelligenceReport.PrimarySource.IntelligenceEvaluation = "A - Known directly";
                        returnIntelligenceReport.NumberOfOtherSources = 0;
                        returnIntelligenceReport.SecondarySource.Clear();
                        for (int i = 0; i < returnIntelligenceReport.NumberOfOtherSources; i++)
                        {
                            returnIntelligenceReport.SecondarySource.Add(ReturnSecondarySource(irName + i));
                        }
                        returnIntelligenceReport.RiskAssessment.CreateRiskAssessment = "risk assessment yes";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterial = "confidential material yes";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterialYes.CategoryOfMaterial = "Funny material";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterialYes.Restrictions = "Do not look at the light!";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Receipt = "REC123";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Dissemination = "I DISS this!";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Use = "It can be?";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.PurposeDissemination = "What can I want it for";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SpecialConditionsForRetention = "retention yes";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SpecialConditionsForRetentionDetails = "These conditions";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskManagementPlan = "There is a risk, and there is a plan!";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.LevelOfRisk = "Low";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.ReviewDate = "TODAY+20";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskProtectiveMarking.Level = "protective marking offical-sensitive";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskProtectiveMarking.ReasonForChange = null;
                        break;
                    }
                case "test simple":
                    {
                        returnIntelligenceReport.Title = irName;
                        returnIntelligenceReport.DateOfIntelligenceCollection = "TODAY";
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.ThreatValues.Add("Cross-Cutting|Prisons and lifetime management");
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.HOCCValues.Clear();
                        returnIntelligenceReport.IntelligenceReportSupportValuesModel.POAValues.Clear();
                        returnIntelligenceReport.HeaderConfidenceLevel = "confidence level low";
                        returnIntelligenceReport.ProtectiveMarking.Level = "protective marking official-sensitive";
                        returnIntelligenceReport.ProtectiveMarking.ReasonForChange = null;
                        returnIntelligenceReport.HandlingCode.Code = "P";
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.DetailedHandlingInstructions = null;
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.ActionCode = null;
                        returnIntelligenceReport.HandlingCode.LawfulSharingPermitted.SanitisationCode = null;
                        returnIntelligenceReport.SensitiveSouce.Subtype = "Person";
                        returnIntelligenceReport.SensitiveSouce.SourceReference = "Reference of the source";
                        returnIntelligenceReport.PrimarySource.IntelligenceDetails = "Primary Source Details";
                        returnIntelligenceReport.PrimarySource.SourceEvaluation = "source evaluation 1 - reliable";
                        returnIntelligenceReport.PrimarySource.IntelligenceEvaluation = "A - Known directly";
                        returnIntelligenceReport.NumberOfOtherSources = 0;
                        returnIntelligenceReport.SecondarySource.Clear();
                        for (int i = 0; i < returnIntelligenceReport.NumberOfOtherSources; i++)
                        {
                            returnIntelligenceReport.SecondarySource.Add(ReturnSecondarySource(irName + i));
                        }
                        returnIntelligenceReport.RiskAssessment.CreateRiskAssessment = "risk assessment no";
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterial = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterialYes.CategoryOfMaterial = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SensitiveMaterialYes.Restrictions = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Receipt = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Dissemination = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.Use = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.PurposeDissemination = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SpecialConditionsForRetention = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.SpecialConditionsForRetentionDetails = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskManagementPlan = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.LevelOfRisk = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.ReviewDate = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskProtectiveMarking.Level = null;
                        returnIntelligenceReport.RiskAssessment.RiskAssessmentYes.RiskProtectiveMarking.ReasonForChange = null;
                        break;
                    }
            }

            return returnIntelligenceReport;
        }



    }
}
