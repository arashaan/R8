﻿using R8.Lib.Enums;
using R8.Lib.Localization;
using R8.Lib.MethodReturn;

using System.Collections.Generic;
using System.Globalization;

using Xunit;

namespace R8.Lib.Test.MethodTests
{
    public class ResponseDatabaseTests
    {
        private readonly Localizer _localizer;

        private static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo("tr");
        private static string FolderPath => "E:\\Work\\Develope\\Asp\\Ecohos\\Ecohos.Presentation\\Dictionary";
        private static string JsonFileName => "dic";

        private static List<CultureInfo> SupportedCultures => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("tr"),
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("fa"),
        };

        public ResponseDatabaseTests()
        {
            var configuration = new LocalizerConfiguration
            {
                Folder = FolderPath,
                FileName = JsonFileName
            };
            _localizer = new Localizer(configuration)
            {
                DefaultCulture = DefaultCulture,
                SupportedCultures = SupportedCultures
            };
        }

        [Fact]
        public void CallResponseDatabase_CheckSuccess()
        {
            // Assets
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");

            // Act
            var response = new ResponseDatabase(Flags.Success);

            // Arrange
            Assert.True(response.Success);
        }

        [Fact]
        public void CallResponseDatabase_CheckSuccess2()
        {
            // Assets
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa");

            // Act
            var response = new ResponseDatabase(Flags.Success);
            response.Save = DatabaseSaveState.SaveFailure;

            // Arrange
            Assert.False(response.Success);
        }
    }
}