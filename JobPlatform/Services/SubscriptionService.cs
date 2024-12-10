using AutoMapper;
using JobPlatform.Data;
using JobPlatform.DTO.Subscription;
using JobPlatform.Exceptions;
using JobPlatform.Repositories;

namespace JobPlatform.Services
{
    public class SubscriptionService: ISubscriptionService
    {
        private readonly IUnitOfWork _repositories;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork repositories, IMapper mapper)
        {
            _repositories = repositories;
            _mapper = mapper;
        }

        /**
         * The functions checks whethter there is  already an active subscription and if so whether it has been requested 
         * that it should be replaced be the new one. In this case a new subscription will be created with start day the current one
         * and allowance that of the requested subscription type plus the remaining vacancy post allowance of the canceled subscription.
         * If there is an active subscription but it hasn't been requested that it be replaced the new subscription will start at the 
         * end of the current one.
         * If there is no active subscription a new one will be created starting from today
         */

        public async Task<SubscriptionViewDTO> AddOrRenewSubscription(SubscriptionInsertDTO insertDTO)
        {
            var subscriptionType = await _repositories.SubscriptionTypesRepository.FindById(insertDTO.SubscriptionTypeId);
            var currentSubscription = await _repositories.SubscriptionRepository.FindMemberActiveSubscription(insertDTO.BusinessId);
            Subscription subscription = new();
            

            if (subscriptionType == null) throw new EntityNotFoundException("subscription type");

            subscription.BusinessId = insertDTO.BusinessId;
            if(currentSubscription != null && insertDTO.ReplaceExisting )
            {
                var nextSubscriptions = await _repositories.SubscriptionRepository.FindCurrentAndFutureSubscriptions(insertDTO.BusinessId);
                foreach (var subsc in nextSubscriptions)
                {
                     _repositories.SubscriptionRepository.StopSubscription(subsc);
                }
                subscription.StartDate = DateOnly.FromDateTime(DateTime.Now);
                subscription.EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(subscriptionType.DurationDays));
                subscription.Allowance = currentSubscription.Allowance - await _repositories.VacancyRepository.FindAnnouncementsSinceDate(currentSubscription.StartDate);
                subscription.Allowance += subscriptionType.Allowance;

            }
            else if(currentSubscription !=null)
            {
                subscription.StartDate = currentSubscription.EndDate.AddDays(1);
                subscription.EndDate = subscription.StartDate.AddDays(subscriptionType.DurationDays);
                subscription.Allowance = subscriptionType.Allowance;
            }
            else
            {
                subscription.StartDate = DateOnly.FromDateTime(DateTime.Now);
                subscription.EndDate = subscription.StartDate.AddDays(subscriptionType.DurationDays);
                subscription.Allowance = subscriptionType.Allowance;
            }

            subscription = await _repositories.SubscriptionRepository.AddOne(subscription);
            if(!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SubscriptionViewDTO>(subscription);
            

            

        }

        public async Task<SubscriptionViewDTO> CancelSubscription(long subscriptionId)
        {
            var subscription = await _repositories.SubscriptionRepository.FindById(subscriptionId);
            if (subscription is null) throw new EntityNotFoundException("subscription");
            subscription = _repositories.SubscriptionRepository.StopSubscription(subscription);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SubscriptionViewDTO>(subscription);
        }

        public async Task<SubscriptionViewDTO> CancelSubscription(long subscriptionId, long businessId)
        {
            var subscription = await _repositories.SubscriptionRepository.FindById(subscriptionId);
            if (subscription is null) throw new EntityNotFoundException("subscription");
            if(subscription.BusinessId != businessId) throw new AccessNotAllowedException();
            subscription = _repositories.SubscriptionRepository.StopSubscription(subscription);
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return _mapper.Map<SubscriptionViewDTO>(subscription);
        }

        public async Task<bool> DeleteSubscription(long subscriptionId)
        {
            bool result = await _repositories.SubscriptionRepository.Delete(subscriptionId);
            if (!result) throw new EntityNotFoundException("subscription");
            if (!await _repositories.SaveChanges()) throw new UnableToSaveDataException();
            return result;
        }

        public async Task<IEnumerable<SubscriptionViewDTO>> GetBusinessSubscriptions(long businessId)
        {
            var data = await _repositories.SubscriptionRepository.FindMemberSubscriptionHistory(businessId);
            return _mapper.Map<IEnumerable<SubscriptionViewDTO>>(data);
        }

        public async Task<SubscriptionViewDTO> GetSubscription(long subscriptionId)
        {
            var subscription = await _repositories.SubscriptionRepository.FindById(subscriptionId);
            if (subscription is null) throw new EntityNotFoundException("subscription");
            SubscriptionViewDTO result = _mapper.Map<SubscriptionViewDTO>(subscription);
            if (subscription.EndDate >= DateOnly.FromDateTime(DateTime.Now))
            {
                result.VacancyPostsCount = await _repositories.VacancyRepository.FindAnnouncementsSinceDate(subscription.StartDate);
            }
            return result;
        }

        public async Task<SubscriptionViewDTO> Update(SubscriptionUpdateDTO updateDTO)
        {
            Subscription subscription = _mapper.Map<Subscription>(updateDTO);
            var result = await _repositories.SubscriptionRepository.UpdateOne(subscription, subscription.Id);
            if (result is null) throw new EntityNotFoundException("subscription");
            if(! await _repositories.SaveChanges()) throw new UnableToDeleteException();
            return _mapper.Map<SubscriptionViewDTO>(result);
        }


    }
}
