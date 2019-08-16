using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Models.OutputLog;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.User;
using Newtonsoft.Json;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class DraftHelper
    {
        ServerApiCaller api;

        //Regex regexDraftMakePickModel = new Regex(@"<== Draft\.(?:DraftStatus|MakePick).*?({.*?})", RegexOptions.Compiled | RegexOptions.Singleline);

        public DraftHelper(ServerApiCaller api)
        {
            this.api = api;
        }

        public ICollection<CardDraftPick> GetDraftPicksForCards(string userId, ICollection<int> grpIds)
        {
            var apiResponse = api.GetCardsForDraftPick(userId, grpIds);
            return Mapper.Map<ICollection<CardDraftPick>>(apiResponse.CardsForDraft);
        }

        //public ICollection<CardDraftPick> ParseDraftMakePick(string userId, string text)
        //{
        //    var match = regexDraftMakePickModel.Match(text);
        //    if (match.Success)
        //    {
        //        var model = JsonConvert.DeserializeObject<DraftMakePick>(match.Groups[1].Value);
        //        return GetDraftPicksForCards(userId, model.draftPack.Select(i => int.Parse(i)).ToArray());
        //    }
        //    //return Mapper.Map<ICollection<CardDraftPick>>(
        //    //    JsonConvert.DeserializeObject<GetCardsForDraftPickResponse>("{\"cardsForDraft\":[{\"weight\":0.0,\"rating\":\"1.5\",\"description\":\"Most decks won’t be on the hunt for this, but it does a fine job filling out curves for both of the guilds in its color. Plus, the flavor is great—a witness dies, prompting the Boros Legion to investigate.\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"W\"],\"imageArtUrl\":\"/art_crop/front/8/c/8c31b8e5-2349-4119-9dc2-3e41c5364a78.jpg?1538878352\",\"idArena\":68476,\"name\":\"Hunted Witness\",\"imageCardUrl\":\"/border_crop/front/8/c/8c31b8e5-2349-4119-9dc2-3e41c5364a78.jpg?1538878352\"},{\"weight\":0.0,\"rating\":\"N/A\",\"description\":\"N/A\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"U\"],\"imageArtUrl\":\"/art_crop/front/6/b/6bf4dfc0-c58b-4535-b660-54ceaa6e0217.jpg?1562557054\",\"idArena\":66125,\"name\":\"Spell Pierce\",\"imageCardUrl\":\"/border_crop/front/6/b/6bf4dfc0-c58b-4535-b660-54ceaa6e0217.jpg?1562557054\"},{\"weight\":0.0,\"rating\":\"3.5\",\"description\":\"Disfigure is incredibly efficient, and unlike Shock, can team up to take down bigger creatures without the loss of a card. Your 3/3 beats their 4/4 in a fight, and you only spent one mana in the whole exchange. Efficiency plus power is something I always look for, and this is one of the premium removal spells in the set (though note that it’s an uncommon now).\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"B\"],\"imageArtUrl\":\"/art_crop/front/1/8/18069340-a698-4f75-82cc-cc94fcf82184.jpg?1563898883\",\"idArena\":69880,\"name\":\"Disfigure\",\"imageCardUrl\":\"/border_crop/front/1/8/18069340-a698-4f75-82cc-cc94fcf82184.jpg?1563898883\"},{\"weight\":0.0,\"rating\":\"3.0\",\"description\":\"In a base red deck, this is a really sweet card. If you can draft an Izzet deck with a ton of jump-start, you might even go off, but it’s mostly just a cheap 4/4 (given a few turns).\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"R\"],\"imageArtUrl\":\"/art_crop/front/d/8/d8c9c111-fbc7-44e1-94bd-1ca164370623.jpg?1538879565\",\"idArena\":68576,\"name\":\"Runaway Steam-Kin\",\"imageCardUrl\":\"/border_crop/front/d/8/d8c9c111-fbc7-44e1-94bd-1ca164370623.jpg?1538879565\"},{\"weight\":0.0,\"rating\":\"N/A\",\"description\":\"N/A\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"G\"],\"imageArtUrl\":\"/art_crop/front/5/8/581b7327-3215-4a4f-b4ae-d9d4002ba882.jpg?1562736014\",\"idArena\":67440,\"name\":\"Llanowar Elves\",\"imageCardUrl\":\"/border_crop/front/5/8/581b7327-3215-4a4f-b4ae-d9d4002ba882.jpg?1562736014\"},{\"weight\":0.0,\"rating\":\"3.5\",\"description\":\"Boros Challenger is a beating early and still relevant later in the game, which is exactly what Boros is looking for. It can mentor your 1-drops without any help, and if you have mana lying around, can even teach larger creatures after you pump it.\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"R\",\"W\"],\"imageArtUrl\":\"/art_crop/front/5/4/545f3a30-7984-4046-8a14-51bc9cbc3fe0.jpg?1538879998\",\"idArena\":68617,\"name\":\"Boros Challenger\",\"imageCardUrl\":\"/border_crop/front/5/4/545f3a30-7984-4046-8a14-51bc9cbc3fe0.jpg?1538879998\"},{\"weight\":615.3,\"rating\":\"3.0\",\"description\":\"Yarok is a decent splash, as a 3/5 with deathtouch and lifelink has a big impact on the board. If your mana is good enough (3+ free sources), I’d play Yarok without any ETB abilities, and once you have a couple good creatures with ETB abilities, Yarok’s value goes up meaningfully.\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"B\",\"G\",\"U\"],\"imageArtUrl\":\"/art_crop/front/a/1/a1001d43-e11b-4e5e-acd4-4a50ef89977f.jpg?1563899734\",\"idArena\":70005,\"name\":\"Yarok, the Desecrated\",\"imageCardUrl\":\"/border_crop/front/a/1/a1001d43-e11b-4e5e-acd4-4a50ef89977f.jpg?1563899734\"},{\"weight\":2373.3,\"rating\":\"1.0\",\"description\":\"Niv is just a little too hard to realistically cast, as all five colors is quite the ask. I would need 5+ multicolor fixers to even attempt this, and even then, it’s a hard-to-cast 6/6 flyer that may draw you a card. That’s a lot of effort for not a huge payoff—save this one for showing off.\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[\"B\",\"G\",\"R\",\"U\",\"W\"],\"imageArtUrl\":\"/art_crop/front/5/6/56a2609d-b535-400b-81d9-72989a33c70f.jpg?1559959440\",\"idArena\":69657,\"name\":\"Niv-Mizzet Reborn\",\"imageCardUrl\":\"/border_crop/front/5/6/56a2609d-b535-400b-81d9-72989a33c70f.jpg?1559959440\"},{\"weight\":0.0,\"rating\":\"N/A\",\"description\":\"N/A\",\"topRank\":{\"rank\":0,\"name\":\"\"},\"colors\":[],\"imageArtUrl\":\"/art_crop/front/1/d/1d65d20c-09e5-4139-838b-7e0e48eb2b2b.jpg?1562732269\",\"idArena\":67538,\"name\":\"Helm of the Host\",\"imageCardUrl\":\"/border_crop/front/1/d/1d65d20c-09e5-4139-838b-7e0e48eb2b2b.jpg?1562732269\"}]}").CardsForDraft
        //    //);

        //    return null;
        //}
    }
}
