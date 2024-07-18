using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
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
        public Guid? MergeTagID { get; set; }
    }

    public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
    {
        public DeleteTagCommandValidator(MeWhenDBContext _DB)
        {
            RuleFor(x => x.ID).NotEmpty();
            RuleFor(x => x.MergeTagID)
                .Must(x => _DB.Set<TagModel>().Any(y => y.ID == (x ?? Guid.Empty)))
                .WithMessage("Merge tag ID not found")
                .When(x => x.MergeTagID.HasValue);
        }
    }

    public class DeleteTagCommandHandler(MeWhenDBContext _DB, IAuthUtilities _Auth) : IRequestHandler<DeleteTagCommand>
    {
        public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var el = _DB.Set<TagModel>().FirstOrDefault(x => x.ID == request.ID) ?? throw new BadRequestException("Tag not found");
            el.DateUp = DateTime.UtcNow;
            el.Deleted = true;
            el.UserUp = _Auth.GetUserID();

            // Jika ada ID merge, maka gambar yang punya tag ini akan dipindahkan ke tag lain
            // Tidak perlu migrate kl dia udah ada tag merge
            if (request.MergeTagID.HasValue)
            {
                var hasNewTag = _DB.Set<ImageTagModel>().IsNotDeletedAnd(x => x.TagID == request.MergeTagID).Select(x => x.ImageID);
                var tagMap = _DB.Set<ImageTagModel>().IsNotDeletedAnd(
                    x => hasNewTag.Contains(x.ImageID) && x.TagID == request.MergeTagID
                ).ToList();
                foreach(var t in tagMap) {
                    t.DateUp = DateTime.UtcNow;
                    t.UserUp = _Auth.GetUserID();
                    t.TagID = request.MergeTagID.Value;
                }
                _DB.Update(tagMap);
            }

            _DB.Update(el);
            await _DB.SaveChangesAsync(cancellationToken);
        }
    }
}