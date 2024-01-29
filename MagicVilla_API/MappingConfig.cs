﻿using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using System.Runtime;
using System.Runtime.InteropServices;

namespace MagicVilla_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDto>();
            CreateMap<VillaDto, Villa>();

            CreateMap<Villa,VillaCreateDto>().ReverseMap();
            CreateMap<Villa, VillaUpdateDto>().ReverseMap();
        }
    }
}