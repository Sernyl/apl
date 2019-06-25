using System.Collections.Generic;
// Каждый документ — это список токенов. То есть List<string>.
// Вместо этого будем использовать псевдоним DocumentTokens.
// Это поможет избежать сложных конструкций:
// вместо List<List<string>> будет List<DocumentTokens>
using DocumentTokens = System.Collections.Generic.List<string>;

namespace Antiplagiarism
{
    public class LevenshteinCalculator
    {
        public List<ComparisonResult> CompareDocumentsPairwise(List<DocumentTokens> documents)
        {
            var result = new List<ComparisonResult>();
            if (documents.Count < 2) return result;
            for (var i = 0; i < documents.Count - 1; i++)
            {
                for (var j = i + 1; j < documents.Count; j++)
                {
                    result.Add(CompareDocuments(documents[i], documents[j]));
                }
            }

            return result;
        }

        private ComparisonResult CompareDocuments(DocumentTokens firstDocument, DocumentTokens secondDocument)
        {
            var sumDistances = 0d;
            var tokensCount = firstDocument.Count < secondDocument.Count ? secondDocument.Count : firstDocument.Count;
            firstDocument = AddSpaces(firstDocument, tokensCount);
            secondDocument = AddSpaces(secondDocument, tokensCount);
            for (var i = 0; i < tokensCount; i++)
            {
                sumDistances += TokenDistanceCalculator.GetTokenDistance(firstDocument[i], secondDocument[i]);
            }

            return new ComparisonResult(firstDocument, secondDocument, sumDistances);
        }

        private DocumentTokens AddSpaces(DocumentTokens document, int length)
        {
            for (var i = document.Count; i < length; i++)
            {
                document.Add(" ");
            }

            return document;
        }
    }
}