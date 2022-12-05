module "sql-database-instance" {
  source  = "./gcp/sql-database-instance"
  project = var.project
  name    = var.database_name
  # version = "POSTGRES_11" //var.database_version
  region = var.database_region
  tier   = var.database_tier
}

module "Error" {
  source   = "./gcp/cloud-run"
  project  = var.project
  name     = "Error"
  location = var.region
  image    = var.service_image
}

# module "Error" {
#   dynamic "service" {
#     for_each = var.services
#     content {
#       source   = "./gcp/cloud-run"
#       name     = service.service_name
#       location = var.database_region
#       image    = service.service_image
#     }
#   }
# }

module "Index" {
  source   = "./gcp/cloud-run"
  project  = var.project
  name     = "Index"
  location = var.region
  image    = var.service_image
}
