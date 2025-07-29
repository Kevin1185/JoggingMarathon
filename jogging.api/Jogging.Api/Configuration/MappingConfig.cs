using AutoMapper;
using Jogging.Domain.Models;
using Jogging.Persistence.Models.Account;
using Jogging.Persistence.Models.Address;
using Jogging.Persistence.Models.AgeCategory;
using Jogging.Persistence.Models.Competition;
using Jogging.Persistence.Models.CompetitionPerCategory;
using Jogging.Persistence.Models.CompetitionResult;
using Jogging.Persistence.Models.Person;
using Jogging.Persistence.Models.Registration;
using Jogging.Persistence.Models.Result;
using Jogging.Persistence.Models.School;
using Jogging.Persistence.Models.SearchModels.Registration;
using Jogging.Persistence.Models.SearchModels.Result;
using Jogging.Rest.DTOs.AccountDtos.ConfirmDtos;
using Jogging.Rest.DTOs.AccountDtos.PasswordDtos;
using Jogging.Rest.DTOs.AccountDtos.ProfileDtos;
using Jogging.Rest.DTOs.AddressDtos;
using Jogging.Rest.DTOs.AgeCategoryDtos;
using Jogging.Rest.DTOs.CompetitionDtos;
using Jogging.Rest.DTOs.CompetitionPerCategoryDtos;
using Jogging.Rest.DTOs.PaymentDtos;
using Jogging.Rest.DTOs.PersonDtos;
using Jogging.Rest.DTOs.RegistrationDtos;
using Jogging.Rest.DTOs.ResultDtos;
using Jogging.Rest.DTOs.SchoolDtos;
//using Profile = Jogging.Infrastructure.Models.DatabaseModels.Account.Profile;
using Profile = Jogging.Persistence.Models.Account.Profile;

namespace Jogging.Api.Configuration;

public class MappingConfig : AutoMapper.Profile
{
    public MappingConfig()
    {
        // DATABASE MAPPING
        CreateMap<SimplePerson, PersonDom>().ReverseMap();
        CreateMap<AdvancedPerson, PersonDom>().ReverseMap();
        CreateMap<ExtendedPerson, PersonDom>().ReverseMap();



        CreateMap<SimpleSchool, SchoolDom>().ReverseMap();
        CreateMap<ExtendedSchool, SchoolDom>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SchoolId))
            .ReverseMap()
            .ForMember(dest => dest.SchoolId, opt => opt.MapFrom(src => src.Id));

        CreateMap<SimpleAddress, AddressDom>().ReverseMap();
        CreateMap<ExtendedAddress, AddressDom>().ReverseMap();

        CreateMap<Profile, ProfileDom>().ReverseMap();

        CreateMap<SimpleCompetition, CompetitionDom>().ReverseMap();
        CreateMap<ExtendedCompetition, CompetitionDom>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        CreateMap<CompetitionDom,ExtendedCompetition>();
        CreateMap<ExtendedCompetition, SimpleCompetition>().ReverseMap();

        CreateMap<SimpleCompetitionResult, ResultDom>().ReverseMap();
        CreateMap<ExtendedCompetitionResult, ResultDom>().ReverseMap();
        CreateMap<SimpleResult, ResultDom>().ReverseMap();
        CreateMap<ExtendedResult, ResultDom>().ReverseMap();

        CreateMap<SimpleCompetitionpercategory, CompetitionPerCategoryDom>().ReverseMap();
        CreateMap<ExtendedCompetitionpercategory, CompetitionPerCategoryDom>().ReverseMap();
        CreateMap<CompetitionResultCompetitionPerCategory, CompetitionPerCategoryDom>().ReverseMap();

        CreateMap<SimpleRegistration, RegistrationDom>().ReverseMap();
        CreateMap<ExtendedRegistration, RegistrationDom>().ReverseMap();
        CreateMap<ExtendedRegistrationSearchByPerson, RegistrationDom>().ReverseMap();
        CreateMap<PersonRegistration, RegistrationDom>().ReverseMap();

        CreateMap<SimpleAgeCategory, AgeCategoryDom>().ReverseMap();

        CreateMap<ExtendedRegistration, RegistrationResponseDTO>().ReverseMap();

        // DTO MAPPING
        CreateMap<ConfirmTokenDto, ConfirmTokenDom>().ReverseMap();

        CreateMap<PersonDom, SimplePersonDom>().ReverseMap();
        CreateMap<PersonDom, PersonEmailChangeRequestDto>().ReverseMap();
        CreateMap<PersonDom, PersonResponseDTO>().ReverseMap();
        CreateMap<PersonDom, PersonRequestDTO>()
            .ForMember(dest => dest.School, opt => opt.MapFrom(src => src.School))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            
            .ReverseMap()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()))
            .ForMember(dest => dest.School, opt => opt.MapFrom(src => src.School))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<SchoolDom, SchoolResponseDTO>().ReverseMap();
        CreateMap<SchoolDom, SchoolRequestDTO>().ReverseMap();

        CreateMap<AddressDom, AddressResponseDTO>().ReverseMap();
        CreateMap<AddressDom, AddressRequestDTO>().ReverseMap();

        CreateMap<ProfileDom, ProfileResponseDTO>().ReverseMap();
        CreateMap<ProfileDom, ProfileRequestDTO>().ReverseMap();

        CreateMap<CompetitionDom, CompetitionResponseDTO>().ReverseMap();
        CreateMap<CompetitionDom, CompetitionRequestDTO>().ReverseMap();

        CreateMap<ResultFunctionDom, ExtendedResultFunctionResponse>().ReverseMap();
        CreateMap<ResultDom, ResultResponseDTO>().ReverseMap();
        CreateMap<ResultDom, ResultRequestDTO>().ReverseMap();
        CreateMap<ResultDom, ResultRuntimeRequestDto>().ReverseMap();
        CreateMap<ResultDom, CompetitionResultResponseDTO>().ReverseMap();
        CreateMap<ResultDom, CompetitionResultRequestDTO>().ReverseMap();

        CreateMap<CompetitionPerCategoryDom, RegistrationModifyCompetitionPerCategoryDTO>().ReverseMap();
        CreateMap<CompetitionPerCategoryDom, CompetitionPerCategoryResponseDTO>().ReverseMap();
        CreateMap<CompetitionPerCategoryDom, CompetitionPerCategoryRequestDTO>().ReverseMap();

        CreateMap<AgeCategoryDom, AgeCategoryResponseDTO>().ReverseMap();
        CreateMap<AgeCategoryDom, AgeCategoryRequestDTO>().ReverseMap();

        CreateMap<RegistrationDom, RegistrationModifyPaidDTO>().ReverseMap();
        CreateMap<RegistrationDom, RegistrationModifyRunNumberDTO>().ReverseMap();
        CreateMap<RegistrationDom, RegistrationResponseDTO>().ReverseMap();
        CreateMap<RegistrationDom, ResultRegistrationResponseDTO>().ReverseMap();
        CreateMap<RegistrationDom, RegistrationRequestDTO>().ReverseMap();
        CreateMap<RegistrationDom, RegistrationModifyRunNumberDTO>().ReverseMap();

        CreateMap<PasswordChange, PasswordChangeDom>().ReverseMap();
        CreateMap<PasswordChangeDom, PasswordChangeRequestDTO>().ReverseMap();

        CreateMap<PasswordReset, PasswordResetDom>().ReverseMap();
        CreateMap<PasswordResetDom, PasswordResetRequestDTO>().ReverseMap();

        CreateMap<PaymentNotificationDom, PaymentNotificationDTO>().ReverseMap();

        // DATABASE SEARCH MAPPERS
        CreateMap<ExtendedRegistrationSearchByPerson, RegistrationDom>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RegistrationId))
            .ForMember(dest => dest.RunTime, opt => opt.MapFrom(src => src.RunTime))
            .ForMember(dest => dest.RunNumber, opt => opt.MapFrom(src => src.RunNumber))
            .ForMember(dest => dest.CompetitionPerCategoryId, opt => opt.MapFrom(src => src.CompetitionPerCategoryId))
            .ForMember(dest => dest.Paid, opt => opt.MapFrom(src => src.Paid))
            .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.CompetitionId, opt => opt.MapFrom(src => src.CompetitionId))
            .ForMember(dest => dest.CompetitionPerCategory, opt => opt.MapFrom(src => new CompetitionPerCategoryDom() {
                DistanceName = src.DistanceName
            }))
            .ForMember(dest => dest.Person, opt => opt.MapFrom(src => new PersonDom {
                Id = src.PersonId,
                LastName = src.LastName,
                FirstName = src.FirstName,
                BirthDate = src.BirthDate,
                IBANNumber = src.IbanNumber,
                SchoolId = src.SchoolId,
                AddressId = src.AddressId,
                Gender = src.Gender,
                UserId = src.UserId,
                Address = new AddressDom {
                    Id = src.AddressId,
                    Street = src.Street,
                    HouseNumber = src.HouseNumber,
                    City = src.City,
                    ZipCode = src.ZipCode
                }
            }));

        CreateMap<ExtendedPersonSearch, PersonDom>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.IBANNumber, opt => opt.MapFrom(src => src.IbanNumber))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.SchoolId, opt => opt.MapFrom(src => src.SchoolId))
            .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.AddressId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDom() {
                Id = src.AddressId,
                Street = src.Street,
                HouseNumber = src.HouseNumber,
                City = src.City,
                ZipCode = src.ZipCode
            }))
            .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => new ProfileDom() {
                Id = src.UserId,
                Role = src.Role
            }));
    }
}


