﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MOTI
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Database1Entities : DbContext
    {
        public Database1Entities()
            : base("name=Database1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Alternative> Alternative { get; set; }
        public virtual DbSet<Criterion> Criterion { get; set; }
        public virtual DbSet<LPR> LPR { get; set; }
        public virtual DbSet<Mark> Mark { get; set; }
        public virtual DbSet<Result> Result { get; set; }
        public virtual DbSet<Vector> Vector { get; set; }
    }
}
