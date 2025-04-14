create
database nomina;

use
nomina;

create table departamento
(
    id          int primary key auto_increment,
    nombre      varchar(36) unique not null,
    descripcion text
);

create table puesto
(
    id          int primary key auto_increment,
    nombre      varchar(36) unique not null,
    descripcion text
);

create table empleado
(
    id              int primary key auto_increment,
    cedula          varchar(11) unique not null,
    nombre          varchar(36)        not null,
    departamento    int                not null references departamento (id) on delete cascade on update cascade,
    puesto          int                not null references departamento (id) on delete cascade on update cascade,
    salario_mensual decimal(10, 2)     not null check (salario_mensual > -1)
);

create table tipo_de_transaccion
(
    id                 int primary key auto_increment,
    nombre             varchar(36) unique not null,
    operacion          enum('INGRESO', 'DEDUCCION') not null,
    depende_de_salario boolean            not null,
    estado             enum('ACTIVO', 'INACTIVO') not null
);

create table transaccion
(
    id         int primary key auto_increment,
    empleado   int                     not null references empleado (id) on delete cascade on update cascade,
    tipo       int                     not null references tipo_de_transaccion (id) on delete cascade on update cascade,
    fecha      timestamp default now() not null,
    monto      decimal(10, 2)          not null check (monto > -1),
    id_asiento int,
    estado     enum('ACTIVO', 'INACTIVO') not null
);

create table asiento_contable
(
    id          int primary key auto_increment,
    descripcion text,
    periodo     varchar(7) unique not null, -- 2025-01 ####-##
    monto       decimal(10, 2)    not null check (monto > -1),
    estado      enum('ACTIVO', 'INACTIVO') not null
);