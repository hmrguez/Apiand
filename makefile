# Variables
TOOL_NAME = Apiand.Cli
CONFIGURATION = Release
OUTPUT_DIR = ./nupkg
PROJECT_DIR = ./src/Apiand.Cli
PLAYGROUND_DIR = ./.output

# Default target
.PHONY: default
default: install

# Clean the output directory
.PHONY: clean
clean:
	rm -rf $(PLAYGROUND_DIR)
	mkdir -p $(PLAYGROUND_DIR)

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
	
# Demo api
.PHONY: api
api:
	apiand new  \
               		--output $(PLAYGROUND_DIR)/SimpleApi \
               		--name "SimpleApi" \
               		--architecture DDD \
               		--api-type Rest \
               		--db-type Mongo-D-B
               		
# Demo and reinstall
.PHONY: demo-reinstall
demo-reinstall: clean reinstall api
               		
