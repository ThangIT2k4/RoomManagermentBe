// using Integration.Domain.Entities;
// using Integration.Domain.Enums;
//
// namespace Integration.Domain.Tests.Entities;
//
// public sealed class IntegrationConnectionEntityTests
// {
//     [Fact]
//     public void Create_ShouldSetConnectedStatus()
//     {
//         var entity = IntegrationConnectionEntity.Create(
//             "tenant-1",
//             "user-1",
//             IntegrationProvider.GoogleSheets,
//             "ext-1");
//
//         Assert.NotEqual(Guid.Empty, entity.Id);
//         Assert.Equal(IntegrationConnectionStatus.Connected, entity.Status);
//     }
//
//     [Fact]
//     public void MarkDisconnected_ShouldSetStatusAndUpdatedAt()
//     {
//         var entity = IntegrationConnectionEntity.Create(
//             "tenant-1",
//             "user-1",
//             IntegrationProvider.GoogleSheets,
//             "ext-1");
//
//         entity.MarkDisconnected();
//
//         Assert.Equal(IntegrationConnectionStatus.Disconnected, entity.Status);
//         Assert.NotNull(entity.UpdatedAtUtc);
//     }
//
//     [Fact]
//     public void MarkSynced_ShouldSetSyncDates()
//     {
//         var entity = IntegrationConnectionEntity.Create(
//             "tenant-1",
//             "user-1",
//             IntegrationProvider.GoogleSheets,
//             "ext-1");
//
//         entity.MarkSynced();
//
//         Assert.NotNull(entity.LastSyncedAtUtc);
//         Assert.NotNull(entity.UpdatedAtUtc);
//     }
// }
//
