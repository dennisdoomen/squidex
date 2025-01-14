﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Domain.Apps.Core.Assets;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;

namespace Squidex.Domain.Apps.Entities.Apps.Templates.Builders
{
    public class AssetFieldBuilder : FieldBuilder<AssetFieldBuilder>
    {
        public AssetFieldBuilder(UpsertSchemaFieldBase field, CreateSchema schema)
            : base(field, schema)
        {
        }

        public AssetFieldBuilder MustBeImage()
        {
            Properties<AssetsFieldProperties>(p => p with
            {
                ExpectedType = AssetType.Image
            });

            return this;
        }

        public AssetFieldBuilder MustBe(AssetType type)
        {
            Properties<AssetsFieldProperties>(p => p with
            {
                ExpectedType = type
            });

            return this;
        }

        public AssetFieldBuilder RequireSingle()
        {
            Properties<AssetsFieldProperties>(p => p with
            {
                MaxItems = 1
            });

            return this;
        }
    }
}
