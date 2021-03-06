﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartSql.Versioning.Samples.Entities.Data;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using SmartSql.Versioning;

namespace SmartSql.Versioning.Samples.Entities {
    class Program {
        public static Guid ToGuid(int value) {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        static void Main(string[] args) {
            var Random = new System.Random();

            var EntityAPI = new Data.EntityController();
            var DateOfBirthApi = new Data.EntityDateOfBirthController();
            var BeltSizeApi = new Data.EntityBeltSizeController();

            var StartIndex = EntityAPI.Current().Count() - 1;

            for (int i = 0; i < 100; i++) {
                EntityAPI = new Data.EntityController();

                var Entity = EntityAPI.Add(new Data.Entity() { Name = "Some Person " + i, OwnerUserId = ToGuid(i % 50)});

                DateOfBirthApi = new Data.EntityDateOfBirthController() {
                    Filter_EntityId = Entity.InstanceId,
                    Default_EntityId = Entity.InstanceId,
                };

                BeltSizeApi = new Data.EntityBeltSizeController() {
                    Filter_EntityId = Entity.InstanceId,
                    Default_EntityId = Entity.InstanceId,
                };

                Entity.Name = "Some Person " + i + " Updated";
                EntityAPI.Update(Entity);
                
                DateOfBirthApi.Add(new Data.EntityDateOfBirth() { Value = DateTime.Now });
                DateOfBirthApi.Add(new Data.EntityDateOfBirth() { Value = DateTime.UtcNow });

                BeltSizeApi.Add(new EntityBeltSize() { Value = "Small" });
                BeltSizeApi.Add(new EntityBeltSize() { Value = "Medium" });
                BeltSizeApi.Add(new EntityBeltSize() { Value = "Large" });
            }

            {

                var All = EntityAPI.Current().ToList();
                    
                EntityAPI.Add(new Data.Entity() { Name = "Michael Valenti" });
                EntityAPI.Add(new Data.Entity() { Name = "Rory Valenti" });
            }

            {
                var Rory = EntityAPI.Current().Where(x => x.Name.Contains("Rory")).FirstOrDefault();
                DateOfBirthApi = new Data.EntityDateOfBirthController() {
                    Filter_EntityId = Rory.InstanceId,
                    Default_EntityId = Rory.InstanceId,
                };
                
                var DateOfBirth = DateOfBirthApi.Get();

                DateOfBirthApi.Add(new Data.EntityDateOfBirth() { Value = DateTime.UtcNow });
                var DateOfBirth1 = DateOfBirthApi.Get();

                DateOfBirthApi.Add(new Data.EntityDateOfBirth() { Value = new DateTime(1985,09,18) });
                var DateOfBirth2 = DateOfBirthApi.Get();

            }

      
        }

       

    }
}
