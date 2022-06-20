# Entity Framework Base Package

This NuGet package contains a base entity definition along with an entity service which handles the update, delete, add etc. which automatically sets the properties on the base entity and does some basic validation. Every class is extendable, so you can fit it to your needs. The `Add`, `Delete`, `Update`, and `Restore` functions on the entity services are all `virtual`, so you can extend them to fit your needs if you e.g. need more base attributes set automatically.

The class `EntityBase` is for hard delete entities while `EntitySoftDeleteBase` is for entities which are only soft deleted. There are repositories for each type of entity as well with mostly the same functionality although the soft delete version has a few extra parameters and also has a restore function.

## Entities

### EntityBase
The base entity contains the following properties:

| Name | Type | Description |
| ---- | ---- | ----------- |
| Id | TId | The unique id of the entity |
| Created | DateTime | The UTC time for when this entity was created |
| Updated | DateTime | The UTC time for when this entity was last updated |

### BaseSoftDeleteEntities
This class extends `EntityBase` and adds the following properties:

| Name | Type | Description |
| ---- | ---- | ----------- |
| DeletedAt | DateTime? | The UTC time for when this entity was soft deleted |
| Deleted | bool | Whether the entity is deleted or not |

## Repositories

### EntityRepository
This is the entity service which handles the communication with the database. There is also an interface `IEntityRepository`, so you can use DI to inject it into your code.

| Function | Description |
| -------- | ----------- |
| GetAsync | Fetch a single entity |
| GetListAsync | Get a filtered and possibly paginated list of entities |
| GetListWithSelectAsync | Get a filtered and possibly paginated list of entities where you can specify what properties to return |
| AddAsync | Add a new entity to the DB. The function automatically sets `Created`, `Updated`, and `Id` on the entity |
| UpdateAsync | Update the provided entity in the DB. The function automatically sets `Updated` |
| DeleteAsync | Removes the entity from the DB |

### EntitySoftDeleteRepository
This entity repository handles soft delete entities. It implements the interface `IEntitySoftDeleteRepository` for DI purposes.

For each method in `EntityRepository` where relevant, there are extra parameters for handling soft delete entities e.g. `GetListAsync` where you can choose whether to include deleted in the result list or not (not by default).

Along with the methods from `EntityRepository`, this repository implements the following extra methods along with changed behaviour on `DeleteAsync` including extra parameters regarding to soft delete behaviour on the other functions:

| Function | Description |
| -------- | ----------- |
| DeleteAsync | Soft delete the entity. Sets `Deleted` to `true` and sets `Deleted` as well |
| RestoreAsync | Undelete the entity. Sets `Deleted` to `false` and sets `DeletedAt` to `null` |

## Unit of Work

### UnitOfWork
This Unit of Work design pattern implementation allows to commit and rollback transactions. It implements the interface `IUnitOfWork`.

`EntityRepository`'s `UpdateAsync`, `AddAsync` and `DeleteAsync` do not apply changes. In order to commit transaction and apply changes `UnitOfWork`'s `CommitAsync` has to be called.

| Function    | Description                                                       |
|-------------|-------------------------------------------------------------------|
| CommitAsync | Commit transaction and apply all changes made in tracked entities |
| Rollback    | Revert all changes made in tracked entities                       |
