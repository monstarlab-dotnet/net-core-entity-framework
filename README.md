# Entity Framework

This NuGet package contains a base entity definition along with an entity service which handles the update, soft delete, add etc. which automatically sets the properties on the base entity and does some basic validation. Every class is extendable, so you can fit it to your needs. The `Add`, `Delete`, `Update`, and `Restore` functions on the entity service are all `virtual`, so you can extend them to fit your needs if you e.g. need more base attributes set automatically.

## BaseEntity
The base entity contains the following properties:

| Name | Type | Description |
| ---- | ---- | ----------- |
| Id | Guid? | The unique id of the entity |
| Created | DateTime | The UTC time for when this entity was created |
| Updated | DateTime | The UTC time for when this entity was last updated |
| DeletedAt | DateTime? | The UTC time for when this entity was soft deleted |
| Deleted | bool | Whether the entity is deleted or not |

## EntityRepository
This is the entity service which handles the communication with the database. There is also an interface `IEntityRepository`, so you can use DI to inject it into your code.

| Function | Description |
| -------- | ----------- |
| Get | Fetch a single entity |
| GetList | Get a filtered and possibly paginated list of entities |
| Add | Add a new entity to the DB. The function automatically sets `Created`, `Updated`, and `Id` on the entity |
| Update | Update the provided entity in the DB. The function automatically sets `Updated` |
| Delete | Soft delete the entity. Sets `Deleted` to `true` and sets `Deleted` as well |
| Restore | Undelete the entity. Sets `Deleted` to `false` and sets `DeletedAt` to `null` |