# 🤝 Contributing to OmniServices.Packages.DataBase

Thank you for your interest in contributing to **OmniSql**! Your help makes this project better for everyone. Whether you’re fixing bugs, improving documentation, or adding new features, your contributions are welcome.

---

## 🚀 Why Contribute?

- **Impact:** Used by .NET developers for robust, cross-database operations.
- **Growth:** Learn best practices in database abstraction, migrations, and logging.
- **Community:** Join a growing group of contributors and users.
- **Recognition:** Get featured in our contributors list!

---

## 📝 How to Contribute

1. **Fork the Repository**
   - Click the `Fork` button on [GitHub](https://github.com/erbibeksah/OmniSql).

2. **Clone Your Fork**
   ```bash
   git clone https://github.com/<your-username>/OmniSql.git
   cd OmniSql
   ```

3. **Create a Feature Branch**
   ```bash
   git checkout -b amazing-feature
   ```

4. **Make Your Changes**
   - Follow the [Coding Guidelines](#coding-guidelines).
   - Add tests if applicable.
   - Update documentation if needed.

5. **Commit Your Changes**
   ```bash
   git commit -m "Add amazing feature"
   ```

6. **Push to Your Fork**
   ```bash
   git push origin amazing-feature
   ```

7. **Open a Pull Request**
   - Go to your fork on GitHub and click `New Pull Request`.
   - Fill out the PR template and describe your changes.

---

## 🛠️ Coding Guidelines

- **.NET 10 / C# 12**: Use modern language features.
- **GlobalUsings.cs:** Centralize your `using` directives for cleaner code. Add `global using DataBase;` in your project root for instant access to OmniSql nuget package Uses.
- **Centralized Package Management:** Use the solution-level `Directory.Packages.props` or your `.csproj` for NuGet dependencies. Keep packages up-to-date and consistent.
- **Configuration Practices:** Store settings in `appsettings.json` (e.g., connection strings, provider names). Use `AppSettingFile.Initialize(builder.Configuration);` for loading configuration globally.
- **Naming:** Use clear, descriptive names.
- **Documentation:** Add XML comments for public methods.
- **Testing:** Add/modify unit tests for new features or bug fixes.
- **Formatting:** Use [EditorConfig](https://editorconfig.org/) standards.

<!-- --- -->

<!-- ## 🧪 Running Tests

OmniSql uses standard .NET unit tests. To run tests:

```bash
dotnet test
``` -->

---

## 💡 Feature Ideas

- Support for additional databases (MySQL, Oracle)
- Advanced migration scenarios
- More logging providers
- Performance improvements

Feel free to open an issue to discuss your ideas!

---

## 🐞 Reporting Bugs

1. Search [existing issues](https://github.com/erbibeksah/OmniSql/issues).
2. If not found, open a new issue with:
   - Steps to reproduce
   - Expected vs. actual behavior
   - Environment details (.NET version, OS, DB provider)

---

## 📚 Resources

- [README.md](./README.md) — Setup, usage, and API documentation
- [LICENSE](./LICENSE) — MIT License
- [FluentMigrator Docs](https://fluentmigrator.github.io/)
- [Serilog Docs](https://serilog.net/)

---

## 🌟 Contributors

Thanks to everyone who has contributed!  
Want to see your name here? Submit your first PR!

---

<div align="center">

<b>Made with ❤️ and ☕ by <a href="https://github.com/erbibeksah">erbibeksah</a></b>  
<i>If OmniSql saved you time, please give it a ⭐ on <a href="https://github.com/erbibeksah/OmniSql">GitHub</a>!</i>

</div>