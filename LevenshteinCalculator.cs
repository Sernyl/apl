using System;
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
            return new ComparisonResult(firstDocument, secondDocument, GetMin(firstDocument, secondDocument));
        }

        private double GetMin(DocumentTokens firstDocument, DocumentTokens secondDocument)
        {
            if (firstDocument.Count > secondDocument.Count)
                return GetMinDistance(new Document(secondDocument, 0), new Document(firstDocument, 0));

            if (firstDocument.Count < secondDocument.Count)
                return GetMinDistance(new Document(firstDocument, 0), new Document(secondDocument, 0));

            var first = GetMinDistance(new Document(secondDocument, 0), new Document(firstDocument, 0));
            var second = GetMinDistance(new Document(firstDocument, 0), new Document(secondDocument, 0));
            return Math.Min(first, second);
        }

        private double GetMinDistance(Document minDocument, Document maxDocument)
        {
            if (minDocument.IsEnd)
                return maxDocument.Length - maxDocument.Index;

            if (maxDocument.IsEnd)
                return minDocument.Length - minDocument.Index;

            var tokenDistance = TokenDistanceCalculator.GetTokenDistance(minDocument.Current, maxDocument.Current);
            if (tokenDistance < 1)
            {
                return GetMinDistance(minDocument.GetNext, maxDocument.GetNext) + tokenDistance;
            }

            var addTokenDistance = GetMinDistance(minDocument, maxDocument.GetNext) + 1;
            var replaceTokenDistance = GetMinDistance(minDocument.GetNext, maxDocument.GetNext) + 1;

            return Math.Min(addTokenDistance, replaceTokenDistance);
        }

        private class Document
        {
            public Document(DocumentTokens tokens, int index)
            {
                Tokens = tokens;
                Index = index;
            }

            public Document GetNext => new Document(Tokens, Index + 1);
            public DocumentTokens Tokens { get; }
            public int Index { get; }
            public int Length => Tokens.Count;
            public bool IsEnd => Index == Tokens.Count;
            public string Current => Tokens[Index];
            public override string ToString()
            {
                return $"{Current} [{Index}]";
            }
        }
    }
}