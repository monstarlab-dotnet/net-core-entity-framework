# Entity Framework Base Package

This NuGet package contains a base entity definition along with an entity service which handles the update, delete, add etc. which automatically sets the properties on the base entity and does some basic validation. Every class is extendable, so you can fit it to your needs. The `Add`, `Delete`, `Update`, and `Restore` functions on the entity services are all `virtual`, so you can extend them to fit your needs if you e.g. need more base attributes set automatically.

The class `BaseEntity` is for hard delete entities while `BaseSoftDeleteEntity` is for entities which are only soft deleted. There are repositories for each type of entity as well with mostly the same functionality although the soft delete version has a few extra parameters and also has a restore function.

## Entities

### BaseEntity
The base entity contains the following properties:

| Name | Type | Description |
| ---- | ---- | ----------- |
| Id | Guid? | The unique id of the entity |
| Created | DateTime | The UTC time for when this entity was created |
| Updated | DateTime | The UTC time for when this entity was last updated |

### BaseSoftDeleteEntities
This class extends `BaseEntity` and adds the following properties:

| Name | Type | Description |
| ---- | ---- | ----------- |
| DeletedAt | DateTime? | The UTC time for when this entity was soft deleted |
| Deleted | bool | Whether the entity is deleted or not |

## Repositories

### EntityRepository
This is the entity service which handles the communication with the database. There is also an interface `IEntityRepository`, so you can use DI to inject it into your code.

| Function | Description |
| -------- | ----------- |
| Get | Fetch a single entity |
| GetList | Get a filtered and possibly paginated list of entities |
| GetListWithSelect | Get a filtered and possibly paginated list of entities where you can specify what properties to return |
| Add | Add a new entity to the DB. The function automatically sets `Created`, `Updated`, and `Id` on the entity |
| Update | Update the provided entity in the DB. The function automatically sets `Updated` |
| Delete | Removes the entity from the DB |

### EntitySoftDeleteRepository
This entity repository handles soft delete entities. It implements the interface `IEntitySoftDeleteRepository` for DI purposes.

For each method in `EntityRepository` where relevant, there are extra parameters for handling soft delete entities e.g. `GetList` where you can choose whether to include deleted in the result list or not (not by default).

Along with the methods from `EntityRepository`, this repository implements the following extra methods along with changed behaviour on `Delete`:

| Function | Description |
| -------- | ----------- |
| Delete | Soft delete the entity. Sets `Deleted` to `true` and sets `Deleted` as well |
| Restore | Undelete the entity. Sets `Deleted` to `false` and sets `DeletedAt` to `null` |