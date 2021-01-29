using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.Services
{
    public class BasicLandIdentifier
    {
        public bool IsBasicLand(Card card)
        {
            return card.type.StartsWith("Basic Land") || card.type.StartsWith("Basic Snow Land");
        }
    }
}
