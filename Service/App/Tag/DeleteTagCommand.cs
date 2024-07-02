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
    public class DeleteTagCommand : IRequest
    {
        public required Guid ID { get; set; }
    }

    public class DeleteTagCommandHandler(MeWhenDBContext _DB, IAuthUtilities _Auth) : IRequestHandler<DeleteTagCommand>
    {
        public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var el = _DB.Set<TagModel>().FirstOrDefault(x => x.ID == request.ID) ?? throw new BadRequestException("Tag not found");
            el.DateUp = DateTime.UtcNow;
            el.Deleted = true;
            el.UserUp = _Auth.GetUserID();
            _DB.Update(el);
            await _DB.SaveChangesAsync(cancellationToken);
        }
    }
}