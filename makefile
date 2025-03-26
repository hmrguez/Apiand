# Variables
TOOL_NAME = Apiand.Cli
CONFIGURATION = Release
OUTPUT_DIR = ./nupkg
PROJECT_DIR = ./src/Apiand.Cli
PLAYGROUND_DIR = ./.output
DEMO_NAME = SimpleApi

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
	apiand new ddd \
               		--output $(PLAYGROUND_DIR)/$(DEMO_NAME) \
               		--name $(DEMO_NAME) \
               		--api-type Fast-Endpoints \
               		--db-type Mongo-D-B
               		
# Demo and reinstall
.PHONY: demo-reinstall
demo-reinstall: clean reinstall api
               		
               		
# Add service
.PHONY: add-service
add-service: demo-reinstall
	apiand generate service Orders.User -p $(PLAYGROUND_DIR)/$(DEMO_NAME)

# Add endpoint
.PHONY: add-endpoint
add-endpoint: demo-reinstall
	apiand generate endpoint GetDemo -p $(PLAYGROUND_DIR)/$(DEMO_NAME) --http-method Put

# Add entity
.PHONY: add-entity
add-entity: demo-reinstall
	apiand generate entity orders.Customer -p $(PLAYGROUND_DIR)/$(DEMO_NAME) --attributes "name:string,email:string,status:enum[active,inactive]"

# Verify which performs dotnet restore
.PHONY: verify
verify:
	dotnet build $(PLAYGROUND_DIR)/$(DEMO_NAME)
