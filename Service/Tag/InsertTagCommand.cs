using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MeWhen.Domain.Model;
using MeWhen.Infrastructure.Context;

namespace MeWhen.Service.Tag
{
    public class InsertTagCommand : IRequest
    {
        public required List<string> Tags { get; set; }
    }

    public class InsertTagCommandHandler(MeWhenDBContext _DB) : IRequestHandler<InsertTagCommand>
    {
        public async Task Handle(InsertTagCommand request, CancellationToken cancellationToken)
        {
            var toBeInserted = _DB.Set<TagModel>()
                .Where(x => !request.Tags.Contains(x.Name))
                .Select(x => x.Name)
                .ToList();

            await _DB.Set<TagModel>().AddRangeAsync(toBeInserted.Select(x => new TagModel() {
                Name = x,
                AgeRating = Domain.Constant.ModelConstant.AgeRating.GENERAL,
                ID = Guid.NewGuid()
            }), cancellationToken);

            await _DB.SaveChangesAsync(cancellationToken);
        }
    }
}