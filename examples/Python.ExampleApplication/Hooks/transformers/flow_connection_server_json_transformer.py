from tableau_migration import (
    IPublishableFlow,
    JsonContentTransformerBase
)


class FlowConnectionServerJsonTransformer(JsonContentTransformerBase[IPublishableFlow]):

    def needs_json_transforming(self, ctx: IPublishableFlow) -> bool:
        # Returning false prevents the transform method from running.
        # Implementing this method potentially allows flows to migrate
        # without loading the file into memory, improving migration speed.
        return len(ctx.connections) > 0

    def transform(self, ctx: IPublishableFlow, json_obj: dict) -> None:
        # Changes to the JSON are saved back to the flow file before publishing.
        for _, connection in json_obj.get("connections", {}).items():
            attrs = connection.get("connectionAttributes", {})
            if attrs.get("server") == "https://old-server":
                attrs["server"] = "https://new-server"
