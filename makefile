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
	apiand new ddd --output $(PLAYGROUND_DIR)/$(DEMO_NAME) --name $(DEMO_NAME) \
               		--presentation FastEndpoints \
               		--application MediatR \
               		--infrastructure EFCore
               		
# Demo and reinstall
.PHONY: demo-reinstall
demo-reinstall: clean reinstall api

# DEMO and reinstall single layer
.PHONY: demo-reinstall-single-layer
demo-reinstall-single-layer: clean reinstall
	apiand new single-layer \
			   		--output $(PLAYGROUND_DIR)/$(DEMO_NAME) \
			   		--name $(DEMO_NAME) \
               		
# Add service
.PHONY: add-service
add-service: demo-reinstall
	apiand generate service Orders.User -o $(PLAYGROUND_DIR)/$(DEMO_NAME)

# Add endpoint
.PHONY: add-endpoint
add-endpoint: demo-reinstall
	apiand generate endpoint GetDemo -o $(PLAYGROUND_DIR)/$(DEMO_NAME) --http-method Put

# Add entity
.PHONY: add-entity
add-entity: demo-reinstall
	apiand generate entity orders.Customer -o $(PLAYGROUND_DIR)/$(DEMO_NAME) --attributes "name:string;email:string;status:enum[active,inactive]"

# Add service single layer
.PHONY: add-service-single-layer
add-service-single-layer: demo-reinstall-single-layer
	apiand generate service Orders.Shipment -o $(PLAYGROUND_DIR)/$(DEMO_NAME)

# Add endpoint single layer
.PHONY: add-endpoint-single-layer
add-endpoint-single-layer: demo-reinstall-single-layer
	apiand generate endpoint GetDemo -o $(PLAYGROUND_DIR)/$(DEMO_NAME) --http-method Put

# Add entity single layer
.PHONY: add-entity-single-layer
add-entity-single-layer: demo-reinstall-single-layer
	apiand generate entity orders.Customer -o $(PLAYGROUND_DIR)/$(DEMO_NAME) --attributes "name:string;email:string;status:enum[active,inactive]"

# Verify which performs dotnet restore
.PHONY: verify
verify:
	dotnet build $(PLAYGROUND_DIR)/$(DEMO_NAME)

# Init
.PHONY: init
init: clean reinstall
	apiand init --output $(PLAYGROUND_DIR)/
	
.PHONY: blank
blank: clean reinstall
	apiand new ddd --output $(PLAYGROUND_DIR)/$(DEMO_NAME) --name $(DEMO_NAME) --skip-interactive
	
.PHONY: microservices
microservices: clean reinstall
	apiand new microservices --output $(PLAYGROUND_DIR)/$(DEMO_NAME) --name $(DEMO_NAME)

