using FluentValidation;
using MediatR;
using MeWhenAPI.Domain.Exception;
using MeWhenAPI.Domain.Model;
using MeWhenAPI.Infrastructure.Context;
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
            var deletedTag = _DB.Set<TagModel>().FirstOrDefault(x => x.ID == request.ID) ?? throw new BadRequestException("Tag not found");

            // Jika ada ID merge, maka gambar yang punya tag ini akan dipindahkan ke tag lain
            // Tidak perlu migrate kl dia udah ada tag merge
            if (request.MergeTagID.HasValue)
            {
                var mergeTag = _DB.Set<TagModel>().FirstOrDefault(x => x.ID == request.MergeTagID.Value) ?? throw new BadRequestException("Merge tag ID not found");
                mergeTag.Alias.Add(deletedTag.Name);

                // Gambar ini punya tag yang merged
                var hasMergeTag = _DB.Set<ImageTagModel>().IsNotDeletedAnd(x => x.TagID == request.MergeTagID).Select(x => x.ImageID);
                
                // Gambar ini hanya punya tag yang deleted. Pindahkan
                var tagMap = _DB.Set<ImageTagModel>()
                    .IsNotDeletedAnd(x => !hasMergeTag.Contains(x.ImageID) && x.TagID == request.ID)
                    .ToList();

                foreach(var t in tagMap) {
                    t.DateUp = DateTime.UtcNow;
                    t.UserUp = _Auth.GetUserID();
                    t.TagID = request.MergeTagID.Value;
                }
                _DB.Update(tagMap);
                _DB.Update(mergeTag);
            }

            // Hapus tag yang mau didelete beserta dependancynya
            _DB.Remove(deletedTag);
            await _DB.SaveChangesAsync(cancellationToken);
        }
    }
}