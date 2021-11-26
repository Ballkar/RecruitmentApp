using AutoMapper;
using Microsoft.Extensions.Logging;
using RecruitmentApp.Entities;
using RecruitmentApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public interface IFeedbackService
    {
        bool AddFeedback(AddFeedbackDto addFeedbackDto);
    }
    public class FeedbackService : IFeedbackService
    {
        private RecruitmentAppDbContext _dbContext;
        private IMapper _mapper;
        private ILogger<FeedbackService> _logger;

        public FeedbackService(RecruitmentAppDbContext dbContext, IMapper mapper, ILogger<FeedbackService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }
        public bool AddFeedback(AddFeedbackDto addFeedbackDto)
        {
            var feedback = _mapper.Map<Feedback>(addFeedbackDto);

            _dbContext.Feedbacks.Add(feedback);
            _dbContext.SaveChanges();
            
            return true;
        }
    }
}
