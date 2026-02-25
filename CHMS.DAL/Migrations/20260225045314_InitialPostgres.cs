using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "amenities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    iconurl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_amenities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    discountpercent = table.Column<int>(type: "integer", nullable: true),
                    discountamount = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    maxdiscountamount = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    minbookingamount = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    startdate = table.Column<DateOnly>(type: "date", nullable: false),
                    enddate = table.Column<DateOnly>(type: "date", nullable: false),
                    maxusage = table.Column<int>(type: "integer", nullable: true),
                    currentusage = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deletedby = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "provinces",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_provinces", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fullname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    passwordhash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    avatarurl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deletedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_users_deletedby",
                        column: x => x.deletedby,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "districts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    provinceid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_districts", x => x.id);
                    table.ForeignKey(
                        name: "fk_districts_provinces_provinceid",
                        column: x => x.provinceid,
                        principalTable: "provinces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rolepermissions",
                columns: table => new
                {
                    roleid = table.Column<Guid>(type: "uuid", nullable: false),
                    permissionid = table.Column<Guid>(type: "uuid", nullable: false),
                    assignedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rolepermissions", x => new { x.roleid, x.permissionid });
                    table.ForeignKey(
                        name: "fk_rolepermissions_permissions_permissionid",
                        column: x => x.permissionid,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rolepermissions_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auditlogs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entityid = table.Column<Guid>(type: "uuid", nullable: true),
                    oldvalues = table.Column<string>(type: "text", nullable: true),
                    newvalues = table.Column<string>(type: "text", nullable: true),
                    ipaddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    useragent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditlogs", x => x.id);
                    table.ForeignKey(
                        name: "fk_auditlogs_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "chathistories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: true),
                    sessionid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    message = table.Column<string>(type: "text", nullable: false),
                    sender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chathistories", x => x.id);
                    table.ForeignKey(
                        name: "fk_chathistories_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    referenceid = table.Column<Guid>(type: "uuid", nullable: true),
                    referencetype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    isread = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    readat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refreshtokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expiresat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    revokedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    replacedbytoken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    deviceinfo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refreshtokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refreshtokens_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userroles",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    roleid = table.Column<Guid>(type: "uuid", nullable: false),
                    assignedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_userroles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_userroles_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_userroles_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verificationdocuments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    documenttype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    documenturl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    reviewedby = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejectreason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    submissiondate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verificationdocuments", x => x.id);
                    table.ForeignKey(
                        name: "fk_verificationdocuments_users_reviewedby",
                        column: x => x.reviewedby,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_verificationdocuments_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    districtid = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_locations_districts_districtid",
                        column: x => x.districtid,
                        principalTable: "districts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "homestays",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ownerid = table.Column<Guid>(type: "uuid", nullable: false),
                    locationid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    pricepernight = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    maxguests = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    bedrooms = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    bathrooms = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    area = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    cancellationpolicy = table.Column<string>(type: "text", nullable: true),
                    houserules = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deletedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_homestays", x => x.id);
                    table.ForeignKey(
                        name: "fk_homestays_locations_locationid",
                        column: x => x.locationid,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_homestays_users_deletedby",
                        column: x => x.deletedby,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_homestays_users_ownerid",
                        column: x => x.ownerid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "amenityhomestay",
                columns: table => new
                {
                    amenitiesid = table.Column<Guid>(type: "uuid", nullable: false),
                    homestaysid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_amenityhomestay", x => new { x.amenitiesid, x.homestaysid });
                    table.ForeignKey(
                        name: "fk_amenityhomestay_amenities_amenitiesid",
                        column: x => x.amenitiesid,
                        principalTable: "amenities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_amenityhomestay_homestays_homestaysid",
                        column: x => x.homestaysid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "availabilities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    isavailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    priceoverride = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_availabilities", x => x.id);
                    table.ForeignKey(
                        name: "fk_availabilities_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    promotionid = table.Column<Guid>(type: "uuid", nullable: true),
                    checkin = table.Column<DateOnly>(type: "date", nullable: false),
                    checkout = table.Column<DateOnly>(type: "date", nullable: false),
                    guestscount = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    pricepernightatbooking = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    totalnights = table.Column<int>(type: "integer", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    discountamount = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    totalprice = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    specialrequests = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    contactphone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deletedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookings", x => x.id);
                    table.ForeignKey(
                        name: "fk_bookings_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_promotions_promotionid",
                        column: x => x.promotionid,
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_bookings_users_customerid",
                        column: x => x.customerid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_users_deletedby",
                        column: x => x.deletedby,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "homestayamenities",
                columns: table => new
                {
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    amenityid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_homestayamenities", x => new { x.homestayid, x.amenityid });
                    table.ForeignKey(
                        name: "fk_homestayamenities_amenities_amenityid",
                        column: x => x.amenityid,
                        principalTable: "amenities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_homestayamenities_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "homestayimages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    imageurl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    caption = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    isprimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    displayorder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_homestayimages", x => x.id);
                    table.ForeignKey(
                        name: "fk_homestayimages_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seasonalpricings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    startdate = table.Column<DateOnly>(type: "date", nullable: false),
                    enddate = table.Column<DateOnly>(type: "date", nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seasonalpricings", x => x.id);
                    table.ForeignKey(
                        name: "fk_seasonalpricings_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wishlists",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wishlists", x => new { x.userid, x.homestayid });
                    table.ForeignKey(
                        name: "fk_wishlists_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_wishlists_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bookingstatushistories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingid = table.Column<Guid>(type: "uuid", nullable: false),
                    oldstatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    newstatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    changedby = table.Column<Guid>(type: "uuid", nullable: true),
                    changedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookingstatushistories", x => x.id);
                    table.ForeignKey(
                        name: "fk_bookingstatushistories_bookings_bookingid",
                        column: x => x.bookingid,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bookingstatushistories_users_changedby",
                        column: x => x.changedby,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cancellations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingid = table.Column<Guid>(type: "uuid", nullable: false),
                    cancelledby = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refundamount = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    refundstatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    refundedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cancellations", x => x.id);
                    table.ForeignKey(
                        name: "fk_cancellations_bookings_bookingid",
                        column: x => x.bookingid,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cancellations_users_cancelledby",
                        column: x => x.cancelledby,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    guestid = table.Column<Guid>(type: "uuid", nullable: false),
                    hostid = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingid = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmessageat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conversations", x => x.id);
                    table.ForeignKey(
                        name: "fk_conversations_bookings_bookingid",
                        column: x => x.bookingid,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_conversations_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_conversations_users_guestid",
                        column: x => x.guestid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_conversations_users_hostid",
                        column: x => x.hostid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingid = table.Column<Guid>(type: "uuid", nullable: false),
                    method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    transactionid = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                    gatewayresponse = table.Column<string>(type: "text", nullable: true),
                    paidat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_payments_bookings_bookingid",
                        column: x => x.bookingid,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    homestayid = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    cleanlinessrating = table.Column<int>(type: "integer", nullable: true),
                    locationrating = table.Column<int>(type: "integer", nullable: true),
                    valuerating = table.Column<int>(type: "integer", nullable: true),
                    communicationrating = table.Column<int>(type: "integer", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    replyfromowner = table.Column<string>(type: "text", nullable: true),
                    replyat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isverified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deletedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviews", x => x.id);
                    table.ForeignKey(
                        name: "fk_reviews_bookings_bookingid",
                        column: x => x.bookingid,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reviews_homestays_homestayid",
                        column: x => x.homestayid,
                        principalTable: "homestays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reviews_users_customerid",
                        column: x => x.customerid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reviews_users_deletedby",
                        column: x => x.deletedby,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supporttickets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    staffid = table.Column<Guid>(type: "uuid", nullable: true),
                    bookingid = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "NORMAL"),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "OPEN"),
                    resolvedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    deletedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supporttickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_supporttickets_bookings_bookingid",
                        column: x => x.bookingid,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_supporttickets_users_staffid",
                        column: x => x.staffid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_supporttickets_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversationid = table.Column<Guid>(type: "uuid", nullable: false),
                    senderid = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    isread = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    readat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_conversations_conversationid",
                        column: x => x.conversationid,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_users_senderid",
                        column: x => x.senderid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviewimages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewid = table.Column<Guid>(type: "uuid", nullable: false),
                    imageurl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reviewimages", x => x.id);
                    table.ForeignKey(
                        name: "fk_reviewimages_reviews_reviewid",
                        column: x => x.reviewid,
                        principalTable: "reviews",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticketreplies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticketid = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticketreplies", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticketreplies_supporttickets_ticketid",
                        column: x => x.ticketid,
                        principalTable: "supporttickets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ticketreplies_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_amenities_name",
                table: "amenities",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_amenityhomestay_homestaysid",
                table: "amenityhomestay",
                column: "homestaysid");

            migrationBuilder.CreateIndex(
                name: "ix_auditlogs_userid",
                table: "auditlogs",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_availabilities_homestayid_date",
                table: "availabilities",
                columns: new[] { "homestayid", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bookings_customerid",
                table: "bookings",
                column: "customerid");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_deletedby",
                table: "bookings",
                column: "deletedby");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_homestayid",
                table: "bookings",
                column: "homestayid");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_promotionid",
                table: "bookings",
                column: "promotionid");

            migrationBuilder.CreateIndex(
                name: "ix_bookingstatushistories_bookingid",
                table: "bookingstatushistories",
                column: "bookingid");

            migrationBuilder.CreateIndex(
                name: "ix_bookingstatushistories_changedby",
                table: "bookingstatushistories",
                column: "changedby");

            migrationBuilder.CreateIndex(
                name: "ix_cancellations_bookingid",
                table: "cancellations",
                column: "bookingid");

            migrationBuilder.CreateIndex(
                name: "ix_cancellations_cancelledby",
                table: "cancellations",
                column: "cancelledby");

            migrationBuilder.CreateIndex(
                name: "ix_chathistories_userid",
                table: "chathistories",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_bookingid",
                table: "conversations",
                column: "bookingid");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_guestid",
                table: "conversations",
                column: "guestid");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_homestayid",
                table: "conversations",
                column: "homestayid");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_hostid",
                table: "conversations",
                column: "hostid");

            migrationBuilder.CreateIndex(
                name: "ix_districts_provinceid",
                table: "districts",
                column: "provinceid");

            migrationBuilder.CreateIndex(
                name: "ix_homestayamenities_amenityid",
                table: "homestayamenities",
                column: "amenityid");

            migrationBuilder.CreateIndex(
                name: "ix_homestayimages_homestayid",
                table: "homestayimages",
                column: "homestayid");

            migrationBuilder.CreateIndex(
                name: "ix_homestays_deletedby",
                table: "homestays",
                column: "deletedby");

            migrationBuilder.CreateIndex(
                name: "ix_homestays_locationid",
                table: "homestays",
                column: "locationid");

            migrationBuilder.CreateIndex(
                name: "ix_homestays_ownerid",
                table: "homestays",
                column: "ownerid");

            migrationBuilder.CreateIndex(
                name: "ix_locations_districtid",
                table: "locations",
                column: "districtid");

            migrationBuilder.CreateIndex(
                name: "ix_messages_conversationid",
                table: "messages",
                column: "conversationid");

            migrationBuilder.CreateIndex(
                name: "ix_messages_senderid",
                table: "messages",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_userid",
                table: "notifications",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_payments_bookingid",
                table: "payments",
                column: "bookingid");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_promotions_code",
                table: "promotions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refreshtokens_userid",
                table: "refreshtokens",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_reviewimages_reviewid",
                table: "reviewimages",
                column: "reviewid");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_bookingid",
                table: "reviews",
                column: "bookingid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reviews_customerid",
                table: "reviews",
                column: "customerid");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_deletedby",
                table: "reviews",
                column: "deletedby");

            migrationBuilder.CreateIndex(
                name: "ix_reviews_homestayid",
                table: "reviews",
                column: "homestayid");

            migrationBuilder.CreateIndex(
                name: "ix_rolepermissions_permissionid",
                table: "rolepermissions",
                column: "permissionid");

            migrationBuilder.CreateIndex(
                name: "ix_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_seasonalpricings_homestayid",
                table: "seasonalpricings",
                column: "homestayid");

            migrationBuilder.CreateIndex(
                name: "ix_supporttickets_bookingid",
                table: "supporttickets",
                column: "bookingid");

            migrationBuilder.CreateIndex(
                name: "ix_supporttickets_staffid",
                table: "supporttickets",
                column: "staffid");

            migrationBuilder.CreateIndex(
                name: "ix_supporttickets_userid",
                table: "supporttickets",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_ticketreplies_ticketid",
                table: "ticketreplies",
                column: "ticketid");

            migrationBuilder.CreateIndex(
                name: "ix_ticketreplies_userid",
                table: "ticketreplies",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_userroles_roleid",
                table: "userroles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "ix_users_deletedby",
                table: "users",
                column: "deletedby");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_verificationdocuments_reviewedby",
                table: "verificationdocuments",
                column: "reviewedby");

            migrationBuilder.CreateIndex(
                name: "ix_verificationdocuments_userid",
                table: "verificationdocuments",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_wishlists_homestayid",
                table: "wishlists",
                column: "homestayid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "amenityhomestay");

            migrationBuilder.DropTable(
                name: "auditlogs");

            migrationBuilder.DropTable(
                name: "availabilities");

            migrationBuilder.DropTable(
                name: "bookingstatushistories");

            migrationBuilder.DropTable(
                name: "cancellations");

            migrationBuilder.DropTable(
                name: "chathistories");

            migrationBuilder.DropTable(
                name: "homestayamenities");

            migrationBuilder.DropTable(
                name: "homestayimages");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "refreshtokens");

            migrationBuilder.DropTable(
                name: "reviewimages");

            migrationBuilder.DropTable(
                name: "rolepermissions");

            migrationBuilder.DropTable(
                name: "seasonalpricings");

            migrationBuilder.DropTable(
                name: "ticketreplies");

            migrationBuilder.DropTable(
                name: "userroles");

            migrationBuilder.DropTable(
                name: "verificationdocuments");

            migrationBuilder.DropTable(
                name: "wishlists");

            migrationBuilder.DropTable(
                name: "amenities");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "supporttickets");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "homestays");

            migrationBuilder.DropTable(
                name: "promotions");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "districts");

            migrationBuilder.DropTable(
                name: "provinces");
        }
    }
}
