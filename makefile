# Variables
TOOL_NAME = Apiand.Cli
CONFIGURATION = Release
OUTPUT_DIR = ./nupkg
PROJECT_DIR = ./src/Apiand.Cli

# Default target
.PHONY: default
default: install

# Clean the output directory
.PHONY: clean
clean:
	rm -rf $(OUTPUT_DIR)
	mkdir -p $(OUTPUT_DIR)

# Build the project
.PHONY: build
build:
	dotnet build $(PROJECT_DIR) -c $(CONFIGURATION)

# Pack the NuGet package
.PHONY: pack
pack: clean
	dotnet pack $(PROJECT_DIR) -c $(CONFIGURATION) -o $(OUTPUT_DIR)

# Uninstall the tool if it exists
.PHONY: uninstall
uninstall:
	-dotnet tool uninstall --global $(TOOL_NAME)

# Install the tool from the local package
.PHONY: install
install: uninstall pack
	dotnet tool install --global --add-source $(OUTPUT_DIR) $(TOOL_NAME)

# Reinstall the tool (uninstall, pack, and install)
.PHONY: reinstall
reinstall: uninstall pack install

# Get the tool version
.PHONY: version
version:
	dotnet tool list --global | grep $(TOOL_NAME)
	
# Demo the tool
.PHONY: demo
demo:
	apiand new --template console --output .output/TempApp --name TempApp