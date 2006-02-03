// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;


	public interface IVisitor
	{
		void VisitModel(ActiveRecordModel model);
		void VisitPrimaryKey(PrimaryKeyModel model);
		void VisitHasManyToAny(HasManyToAnyModel model);
		void VisitAny(AnyModel model);
		void VisitProperty(PropertyModel model);
		void VisitField(FieldModel model);
		void VisitVersion(VersionModel model);
		void VisitTimestamp(TimestampModel model);
		void VisitKey(KeyModel model);
		void VisitBelongsTo(BelongsToModel model);
		void VisitHasMany(HasManyModel model);
		void VisitOneToOne(OneToOneModel model);
		void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model);
		void VisitHilo(HiloModel model);
		void VisitNested(NestedModel model);
		void VisitCollectionID(CollectionIDModel model);
		void VisitHasManyToAnyConfig(HasManyToAnyModel.Config hasManyToAnyConfigModel);
		void VisitImport(ImportModel model);
	}
}
