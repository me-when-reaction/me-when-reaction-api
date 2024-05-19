using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MeWhenAPI.Domain.Constant;
using MeWhenAPI.Domain.Exception;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
using MeWhenAPI.Infrastructure.Helper;
using MeWhenAPI.Infrastructure.Utilities;

namespace MeWhenAPI.Service.App.Tag
{
    public class UpdateTagRatingCommand : IRequest
    {
        public required Guid ID { get; set; }
        public required ModelConstant.AgeRating Rating { get; set; }
    }

    public class UpdateTagRatingCommandHandler(MeWhenDBContext _DB, IAuthUtilities _Auth) : IRequestHandler<UpdateTagRatingCommand>
    {
        public async Task Handle(UpdateTagRatingCommand request, CancellationToken cancellationToken)
        {
            var el = _DB.Set<TagModel>().FirstOrDefault(x => x.ID == request.ID) ?? throw new BadRequestException("Tag not found");
            el.DateUp = DateTime.UtcNow;
            el.UserUp = _Auth.GetUserID();
            el.AgeRating = request.Rating;
            _DB.Update(el);
            await _DB.SaveChangesAsync(cancellationToken);
        }
    }
}