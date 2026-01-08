using System;
using FirebirdSql.EntityFrameworkCore.Firebird.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YusurIntegration.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithNewStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    TokenType = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    AccessToken = table.Column<string>(type: "VARCHAR(2000)", maxLength: 2000, nullable: true),
                    Username = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    Licence = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    IsValid = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "TIMESTAMP", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovedDrugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    ItemNo = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Sfdacode = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    GenericCode = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    FromDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    ToDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    PriorityOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedDrugs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    VendorId = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    BranchLicense = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    ErxReference = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    IsPickup = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Status = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    failureReason = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    DeliveryTimeSlotId = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    DeliveryTimeSlotStartTime = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    DeliveryTimeSlotEndTime = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    DeliveryDate = table.Column<DateOnly>(type: "DATE", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusHistory",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    FailureReason = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistory", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "PendingMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    MessageId = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    BranchLicense = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    MessageType = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    PayloadJson = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    isdelivered = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    PharmacyName = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    LicenseNumber = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Address = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    ContactNumber = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    GroupId = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    isGroup = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    YusurUser = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    YusurPassword = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Code = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PharmacyGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    GroupName = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Description = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    storecode = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    BranchLicense = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    GenericCode = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    ItemNo = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ReorderLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Role = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    RefreshToken = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "TIMESTAMP", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WasfatyDrugs",
                columns: table => new
                {
                    drugId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    version = table.Column<int>(type: "INTEGER", nullable: true),
                    status = table.Column<string>(type: "VARCHAR(12)", maxLength: 12, nullable: true),
                    isInfiniteDivisible = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    strength = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: true),
                    genericName = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    granularUnit = table.Column<int>(type: "INTEGER", nullable: true),
                    updatedDate = table.Column<string>(type: "VARCHAR(12)", maxLength: 12, nullable: true),
                    source = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    packageType = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    division = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    regOwner = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    discontinueDate = table.Column<string>(type: "VARCHAR(12)", maxLength: 12, nullable: true),
                    doseStrengthUnitId = table.Column<int>(type: "INTEGER", nullable: true),
                    isHighAlert = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    price = table.Column<double>(type: "DOUBLE PRECISION", nullable: true),
                    suggestedRouteOfAdmin = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    isAlternative = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    barcode = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    category2 = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    mappingScientificCode = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    routeOfAdmin = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    dosageOrderForm = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    category1 = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    dosageForm = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    dosageFormCode = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    verbSecondLanguageDescription = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    volume = table.Column<double>(type: "DOUBLE PRECISION", nullable: true),
                    isControlled = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    packageVolume = table.Column<double>(type: "DOUBLE PRECISION", nullable: true),
                    volumeUnitId = table.Column<int>(type: "INTEGER", nullable: true),
                    divisibleFactor = table.Column<double>(type: "DOUBLE PRECISION", nullable: true),
                    roaCode = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    doseStrengthUnit = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    maxRefill = table.Column<int>(type: "INTEGER", nullable: true),
                    region = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    verbDescription = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    unitTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    drugCode = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    volumeUnit = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    packageSize = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    unitType = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    tradeName = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    doseStrength = table.Column<double>(type: "DOUBLE PRECISION", nullable: true),
                    ingredients = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    company = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    atcCode = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    isHazardous = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    dosageUnit = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    dosageUnitId = table.Column<int>(type: "INTEGER", nullable: true),
                    isDivisible = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    genericCode = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    isLasa = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    isDelivery = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    isDivisibleFactor = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    createdDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    publishDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    id = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    listId = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    itemId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasfatyDrugs", x => x.drugId);
                });

            migrationBuilder.CreateTable(
                name: "WebhookLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    WebhookType = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    Payload = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    BranchLicense = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    Status = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    BranchConnected = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YusurUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    Licence = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    storename = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    storecode = table.Column<string>(type: "VARCHAR(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YusurUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    ActivityId = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    OrderId = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    GenericCode = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Instructions = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    ArabicInstructions = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    Duration = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Refills = table.Column<int>(type: "INTEGER", nullable: true),
                    SelectedTradeCode = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    SelectedQuantity = table.Column<int>(type: "INTEGER", nullable: true),
                    authStatus = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    rejectionReason = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    Isapproved = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    Itemno = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    nationalId = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    memberId = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    firstName = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    lastName = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    bloodGroup = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    dateOfBirth = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    gender = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    addressLine1 = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    addressLine2 = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    area = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    city = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    Coordinates_latitude = table.Column<double>(type: "DOUBLE PRECISION", nullable: true),
                    Coordinates_longitude = table.Column<double>(type: "DOUBLE PRECISION", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingAddress_Orders_Orde~",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeDrugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Fb:ValueGenerationStrategy", FbValueGenerationStrategy.IdentityColumn),
                    ActivityForeignId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivityId = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Code = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Name = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeDrugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeDrugs_Activities_Activ~",
                        column: x => x.ActivityForeignId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_OrderId_Activity~",
                table: "Activities",
                columns: new[] { "OrderId", "ActivityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_OrderId",
                table: "Patients",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingMessages_MessageId",
                table: "PendingMessages",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddress_OrderId",
                table: "ShippingAddress",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTable_ItemNo_BranchLic~",
                table: "StockTable",
                columns: new[] { "ItemNo", "BranchLicense", "GenericCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeDrugs_ActivityForeignId",
                table: "TradeDrugs",
                column: "ActivityForeignId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiTokens");

            migrationBuilder.DropTable(
                name: "ApprovedDrugs");

            migrationBuilder.DropTable(
                name: "OrderStatusHistory");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "PendingMessages");

            migrationBuilder.DropTable(
                name: "Pharmacies");

            migrationBuilder.DropTable(
                name: "PharmacyGroups");

            migrationBuilder.DropTable(
                name: "ShippingAddress");

            migrationBuilder.DropTable(
                name: "StockTable");

            migrationBuilder.DropTable(
                name: "TradeDrugs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WasfatyDrugs");

            migrationBuilder.DropTable(
                name: "WebhookLogs");

            migrationBuilder.DropTable(
                name: "YusurUsers");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
