using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Application.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVideoResume.Application.Resume;

public partial class ResumeService
{
    public async Task<List<ResumeInformationSummaryDTO>> SearchResumeInformationSummaryData(ResumeSearchRequestDTO searchRequest)
    {
        var result = new List<ResumeInformationSummaryDTO>();
        try
        {
            var query = _dataContext.ResumeInformation
                .Include(x => x.MetaResume).ThenInclude(y => y.Basics)
                .Include(x => x.MetaResume).ThenInclude(y => y.Work)
                .Include(x => x.MetaResume).ThenInclude(y => y.Education)
                .Include(x => x.MetaResume).ThenInclude(y => y.Skills)
                .Include(x => x.MetaData)
                .Include(x => x.UserProfile)
                .Include(x => x.ResumeTemplate)
                .AsNoTracking()
                .Where(x => x.DeletedDateTime == null)
                .Where(x => x.Privacy_ShowResume == DisplayPrivacy.ToPublic)
                .AsSingleQuery();

            if (!string.IsNullOrEmpty(searchRequest.TextQuery))
            {
                var searchTerm = searchRequest.TextQuery.ToLower();
                query = query.Where(x => 
                    x.MetaResume.Basics.Name.ToLower().Contains(searchTerm) ||
                    x.MetaResume.Basics.Summary.ToLower().Contains(searchTerm) ||
                    x.MetaResume.Work.Any(w => w.Position.ToLower().Contains(searchTerm) || 
                                             w.Name.ToLower().Contains(searchTerm) ||
                                             w.Summary.ToLower().Contains(searchTerm)) ||
                    x.MetaResume.Skills.Any(s => s.Name.ToLower().Contains(searchTerm) ||
                                                s.Keywords.Any(k => k.ToLower().Contains(searchTerm))) ||
                    x.MetaResume.Education.Any(e => e.Institution.ToLower().Contains(searchTerm) ||
                                                  e.Area.ToLower().Contains(searchTerm) ||
                                                  e.StudyType.ToLower().Contains(searchTerm)));
            }

            if (searchRequest.Skills != null && searchRequest.Skills.Any())
            {
                foreach (var skill in searchRequest.Skills)
                {
                    var skillLower = skill.ToLower();
                    query = query.Where(x => x.MetaResume.Skills.Any(s => 
                        s.Name.ToLower().Contains(skillLower) ||
                        s.Keywords.Any(k => k.ToLower().Contains(skillLower))));
                }
            }

            if (!string.IsNullOrEmpty(searchRequest.Education))
            {
                var educationTerm = searchRequest.Education.ToLower();
                query = query.Where(x => x.MetaResume.Education.Any(e => 
                    e.Institution.ToLower().Contains(educationTerm) ||
                    e.Area.ToLower().Contains(educationTerm) ||
                    e.StudyType.ToLower().Contains(educationTerm)));
            }

            if (!string.IsNullOrEmpty(searchRequest.Experience))
            {
                var experienceTerm = searchRequest.Experience.ToLower();
                query = query.Where(x => x.MetaResume.Work.Any(w => 
                    w.Position.ToLower().Contains(experienceTerm) ||
                    w.Name.ToLower().Contains(experienceTerm) ||
                    w.Summary.ToLower().Contains(experienceTerm)));
            }

            var resumeItems = await query.Select(x => new ResumeInformationSummaryDTO()
            {
                SentimentScore = x.SentimentScore,
                UserId = x.UserId,
                IsOwner = false,
                CreationDateTimeFormatted = x.CreationDateTime.Value.ToString("yyyy-MM-dd"),
                IsPublic = true,
                Id = x.Id.ToString(),
                TemplateName = x.ResumeTemplate.Name,
                Description = x.MetaResume.Basics.Summary,
                Slug = x.Slug,
                Name = x.MetaResume.Basics.Name,
                IsPrimaryDefault = x.IsPrimaryDefault,
                IsWatched = false,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            }).ToListAsync();

            if (searchRequest.Latitude.HasValue && searchRequest.Longitude.HasValue && searchRequest.RadiusMiles.HasValue)
            {
                resumeItems = SearchService<ResumeInformationSummaryDTO>.GetItemsWithinRadius(
                    resumeItems, 
                    searchRequest.Latitude.Value, 
                    searchRequest.Longitude.Value, 
                    searchRequest.RadiusMiles.Value);
            }

            if (searchRequest.Skip.HasValue)
                resumeItems = resumeItems.Skip(searchRequest.Skip.Value).ToList();
            
            if (searchRequest.Take.HasValue)
                resumeItems = resumeItems.Take(searchRequest.Take.Value).ToList();

            result = resumeItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }
}
