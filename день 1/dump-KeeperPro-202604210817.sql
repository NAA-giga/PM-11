--
-- PostgreSQL database cluster dump
--

-- Started on 2026-04-21 08:17:10

\restrict 5kQcuynX4gkjRZk2l7dUL9ojHd3OqMnLfvVpFNjk86jxIP6WTBIRDEJE8XYXlL8

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

--
-- Roles
--

CREATE ROLE postgres;
ALTER ROLE postgres WITH SUPERUSER INHERIT CREATEROLE CREATEDB LOGIN REPLICATION BYPASSRLS;

--
-- User Configurations
--








\unrestrict 5kQcuynX4gkjRZk2l7dUL9ojHd3OqMnLfvVpFNjk86jxIP6WTBIRDEJE8XYXlL8

--
-- Databases
--

--
-- Database "template1" dump
--

\connect template1

--
-- PostgreSQL database dump
--

\restrict i1H49zyUGh1S8gv0NixhD6vteAHRUkUEuZZqDcGwwcA6nP1FX6cLHGRuzHdjdIg

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

-- Started on 2026-04-21 08:17:10

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

-- Completed on 2026-04-21 08:17:10

--
-- PostgreSQL database dump complete
--

\unrestrict i1H49zyUGh1S8gv0NixhD6vteAHRUkUEuZZqDcGwwcA6nP1FX6cLHGRuzHdjdIg

--
-- Database "KeeperPro" dump
--

--
-- PostgreSQL database dump
--

\restrict fTGEvaxh9eLf9W9B1docXHyyVpH5BhITjpAlvgeVEBrDqyMwrjh4R759DDeEWUT

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

-- Started on 2026-04-21 08:17:11

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4971 (class 1262 OID 16597)
-- Name: KeeperPro; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "KeeperPro" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';


ALTER DATABASE "KeeperPro" OWNER TO postgres;

\unrestrict fTGEvaxh9eLf9W9B1docXHyyVpH5BhITjpAlvgeVEBrDqyMwrjh4R759DDeEWUT
\connect "KeeperPro"
\restrict fTGEvaxh9eLf9W9B1docXHyyVpH5BhITjpAlvgeVEBrDqyMwrjh4R759DDeEWUT

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 229 (class 1255 OID 16844)
-- Name: generate_visitor_login(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.generate_visitor_login() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
    email_prefix TEXT;
    new_login TEXT;
BEGIN
    -- Получаем часть email до символа @
    email_prefix := SPLIT_PART(NEW.email, '@', 1);
    
    -- Очищаем префикс от недопустимых символов
    email_prefix := REGEXP_REPLACE(email_prefix, '[^a-zA-Z0-9]', '', 'g');
    
    -- Формируем логин: часть email + _ + следующий ID
    -- Используем nextval для получения следующего ID
    new_login := email_prefix || '_' || nextval('"Посетитель_id_seq"');
    
    -- Устанавливаем логин
    NEW."Логин" := new_login;
    
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.generate_visitor_login() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 226 (class 1259 OID 16796)
-- Name: Заявка; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Заявка" (
    id integer NOT NULL,
    "Наименование" character varying(255),
    "тип_заявки" character varying(20),
    "Дата_создание_заявки" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "Дата_одобрение_заявки" timestamp without time zone,
    "Цель" text,
    "Статус" character varying(20) DEFAULT 'проверка'::character varying,
    "Дата_начало_реализации_заявки" date CONSTRAINT "Заявка_Дата_начало_реализаци_not_null" NOT NULL,
    "Дата_окончание_реализации_заявки" date CONSTRAINT "Заявка_Дата_окончание_реализ_not_null" NOT NULL,
    "Посетитель_id" integer,
    "Сотрудник_id" integer,
    "Подразделение_id" integer,
    "Причина_отказа" text,
    "Время_входа" timestamp without time zone,
    "Время_выхода" timestamp without time zone,
    CONSTRAINT "Заявка_Статус_check" CHECK ((("Статус")::text = ANY ((ARRAY['проверка'::character varying, 'одобрена'::character varying, 'не одобрена'::character varying])::text[]))),
    CONSTRAINT "Заявка_тип_заявки_check" CHECK ((("тип_заявки")::text = ANY ((ARRAY['личная'::character varying, 'групповая'::character varying])::text[])))
);


ALTER TABLE public."Заявка" OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 16795)
-- Name: Заявка_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Заявка_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Заявка_id_seq" OWNER TO postgres;

--
-- TOC entry 4972 (class 0 OID 0)
-- Dependencies: 225
-- Name: Заявка_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Заявка_id_seq" OWNED BY public."Заявка".id;


--
-- TOC entry 220 (class 1259 OID 16743)
-- Name: Подразделение; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Подразделение" (
    id integer NOT NULL,
    "Наименование" character varying(255) NOT NULL
);


ALTER TABLE public."Подразделение" OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16742)
-- Name: Подразделение_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Подразделение_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Подразделение_id_seq" OWNER TO postgres;

--
-- TOC entry 4973 (class 0 OID 0)
-- Dependencies: 219
-- Name: Подразделение_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Подразделение_id_seq" OWNED BY public."Подразделение".id;


--
-- TOC entry 224 (class 1259 OID 16774)
-- Name: Посетитель; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Посетитель" (
    id integer NOT NULL,
    "Фамилия" character varying(100) NOT NULL,
    "Имя" character varying(100) NOT NULL,
    "Отчество" character varying(100),
    "Телефон" character varying(20),
    email character varying(255),
    "Дата_рождения" date,
    "Примечание" text,
    "Серия_паспорта" character varying(4),
    "Номер_паспорта" character varying(6),
    "Фото_Посетителя" character varying(500),
    "скан_паспорта_посетителя" character varying(500),
    "Организация" character varying(255),
    "Сотрудник_id" integer,
    "Подразделение_id" integer,
    "Пароль" character varying(255) NOT NULL,
    "Логин" character varying(100)
);


ALTER TABLE public."Посетитель" OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16773)
-- Name: Посетитель_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Посетитель_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Посетитель_id_seq" OWNER TO postgres;

--
-- TOC entry 4974 (class 0 OID 0)
-- Dependencies: 223
-- Name: Посетитель_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Посетитель_id_seq" OWNED BY public."Посетитель".id;


--
-- TOC entry 222 (class 1259 OID 16752)
-- Name: Сотрудник; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Сотрудник" (
    id integer NOT NULL,
    "Фамилия" character varying(100) NOT NULL,
    "Имя" character varying(100) NOT NULL,
    "Отчество" character varying(100),
    "Телефон" character varying(20),
    email character varying(255) NOT NULL,
    "Дата_рождения" date,
    "Примечание" text,
    "Логин" character varying(100) NOT NULL,
    "Пароль" character varying(255) NOT NULL,
    "Серия_паспорта" character varying(4),
    "Номер_паспорта" character varying(6),
    "Фото_Посетителя" character varying(500),
    "скан_паспорта_посетителя" character varying(500),
    "Подразделение_id" integer
);


ALTER TABLE public."Сотрудник" OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16751)
-- Name: Сотрудник_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Сотрудник_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Сотрудник_id_seq" OWNER TO postgres;

--
-- TOC entry 4975 (class 0 OID 0)
-- Dependencies: 221
-- Name: Сотрудник_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Сотрудник_id_seq" OWNED BY public."Сотрудник".id;


--
-- TOC entry 228 (class 1259 OID 16848)
-- Name: Черный_список; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Черный_список" (
    id integer NOT NULL,
    "Посетитель_id" integer NOT NULL,
    "Причина" text NOT NULL,
    "Дата_добавления" timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public."Черный_список" OWNER TO postgres;

--
-- TOC entry 4976 (class 0 OID 0)
-- Dependencies: 228
-- Name: COLUMN "Черный_список"."Дата_добавления"; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public."Черный_список"."Дата_добавления" IS 'Дата добавления в черный список';


--
-- TOC entry 227 (class 1259 OID 16847)
-- Name: Черный_список_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Черный_список_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Черный_список_id_seq" OWNER TO postgres;

--
-- TOC entry 4977 (class 0 OID 0)
-- Dependencies: 227
-- Name: Черный_список_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Черный_список_id_seq" OWNED BY public."Черный_список".id;


--
-- TOC entry 4779 (class 2604 OID 16799)
-- Name: Заявка id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Заявка" ALTER COLUMN id SET DEFAULT nextval('public."Заявка_id_seq"'::regclass);


--
-- TOC entry 4776 (class 2604 OID 16746)
-- Name: Подразделение id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Подразделение" ALTER COLUMN id SET DEFAULT nextval('public."Подразделение_id_seq"'::regclass);


--
-- TOC entry 4778 (class 2604 OID 16777)
-- Name: Посетитель id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Посетитель" ALTER COLUMN id SET DEFAULT nextval('public."Посетитель_id_seq"'::regclass);


--
-- TOC entry 4777 (class 2604 OID 16755)
-- Name: Сотрудник id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Сотрудник" ALTER COLUMN id SET DEFAULT nextval('public."Сотрудник_id_seq"'::regclass);


--
-- TOC entry 4782 (class 2604 OID 16851)
-- Name: Черный_список id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Черный_список" ALTER COLUMN id SET DEFAULT nextval('public."Черный_список_id_seq"'::regclass);


--
-- TOC entry 4963 (class 0 OID 16796)
-- Dependencies: 226
-- Data for Name: Заявка; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Заявка" (id, "Наименование", "тип_заявки", "Дата_создание_заявки", "Дата_одобрение_заявки", "Цель", "Статус", "Дата_начало_реализации_заявки", "Дата_окончание_реализации_заявки", "Посетитель_id", "Сотрудник_id", "Подразделение_id", "Причина_отказа", "Время_входа", "Время_выхода") FROM stdin;
13	Проведение переговоров	групповая	2026-04-01 09:30:00	2026-04-02 14:15:00	Обсуждение условий сотрудничества с ООО "ТехноСервис"	одобрена	2026-04-10	2026-04-12	1	3	2	\N	\N	\N
14	Собеседование с кандидатом	личная	2026-04-03 11:00:00	2026-04-04 10:30:00	Проведение технического интервью	одобрена	2026-04-15	2026-04-15	5	4	3	\N	\N	\N
15	Аудиторская проверка	групповая	2026-04-05 14:20:00	2026-04-06 16:45:00	Проверка финансовой отчетности за 1 квартал	одобрена	2026-04-20	2026-04-25	6	5	4	\N	\N	\N
16	Обслуживание оборудования	личная	2026-04-07 08:15:00	\N	Плановое обслуживание серверного оборудования	проверка	2026-04-18	2026-04-18	7	2	2	\N	\N	\N
17	Поставка канцтоваров	личная	2026-04-08 10:00:00	2026-04-09 11:20:00	Доставка офисных принадлежностей	одобрена	2026-04-22	2026-04-22	9	6	5	\N	\N	\N
18	Консультация юриста	личная	2026-04-09 13:45:00	2026-04-10 09:00:00	Правовая консультация по договору аренды	одобрена	2026-04-25	2026-04-25	4	8	7	\N	\N	\N
19	Экскурсия для партнеров	групповая	2026-04-10 15:30:00	\N	Ознакомление с производством	не одобрена	2026-05-05	2026-05-05	10	1	1	\N	\N	\N
20	Техническая поддержка	личная	2026-04-11 09:00:00	2026-04-12 14:00:00	Настройка рабочего места нового сотрудника	одобрена	2026-04-28	2026-04-28	3	3	2	\N	\N	\N
21	Повышение квалификации	групповая	2026-04-12 11:30:00	2026-04-13 15:30:00	Курсы по охране труда	одобрена	2026-05-12	2026-05-14	2	9	8	\N	\N	\N
22	Встреча с ветеранами	групповая	2026-04-13 10:00:00	2026-04-14 12:00:00	Мероприятие ко Дню Победы	одобрена	2026-05-08	2026-05-08	\N	7	6	\N	\N	\N
23	Ремонт оргтехники	личная	2026-04-14 16:20:00	\N	Диагностика и ремонт принтера	проверка	2026-04-29	2026-04-30	7	2	2	\N	\N	\N
24	Совещание с подрядчиками	групповая	2026-04-15 08:45:00	2026-04-16 10:15:00	Планирование строительных работ	одобрена	2026-05-15	2026-05-15	3	1	1	\N	\N	\N
25	Заявка от 17.04.2026	личная	2026-04-17 13:23:49.236654	\N	gfh	проверка	2026-04-19	2026-04-19	1	3	2	\N	\N	\N
26	Заявка от 17.04.2026	личная	2026-04-17 13:41:00.678635	\N	sdfsdf	проверка	2026-04-19	2026-05-04	1	4	3	\N	\N	\N
27	Групповая заявка от 17.04.2026	групповая	2026-04-17 14:05:42.924249	\N	gfghf	проверка	2026-04-19	2026-04-30	1	2	2	\N	\N	\N
28	Заявка от 17.04.2026	личная	2026-04-17 15:47:34.82085	\N	rtyrty	проверка	2026-04-19	2026-04-24	1	1	1	\N	\N	\N
29	Заявка от 20.04.2026	личная	2026-04-20 12:57:49.661263	\N		проверка	2026-04-21	2026-04-22	1	\N	\N	\N	\N	\N
30	Заявка от 20.04.2026	личная	2026-04-20 13:10:58.352804	\N	sdfsdf	проверка	2026-04-21	2026-04-22	1	3	2	\N	\N	\N
\.


--
-- TOC entry 4957 (class 0 OID 16743)
-- Dependencies: 220
-- Data for Name: Подразделение; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Подразделение" (id, "Наименование") FROM stdin;
1	Отдел безопасности
2	IT-департамент
3	Отдел кадров
4	Бухгалтерия
5	Административно-хозяйственный отдел
6	Отдел пропускного режима
7	Юридический отдел
8	Служба охраны труда
\.


--
-- TOC entry 4961 (class 0 OID 16774)
-- Dependencies: 224
-- Data for Name: Посетитель; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Посетитель" (id, "Фамилия", "Имя", "Отчество", "Телефон", email, "Дата_рождения", "Примечание", "Серия_паспорта", "Номер_паспорта", "Фото_Посетителя", "скан_паспорта_посетителя", "Организация", "Сотрудник_id", "Подразделение_id", "Пароль", "Логин") FROM stdin;
2	Кузнецова	Мария	Петровна	+7(916)234-56-78	maria.kuznetsova@yandex.ru	1995-07-14	\N	4513	234567	/photos/visitors/kuznetsova.jpg	/scans/visitors/kuznetsova_passport.pdf	ЗАО "СтройИнвест"	4	3	hash_visitor_2_pass	visitor_2
3	Попов	Денис	Андреевич	+7(916)345-67-89	denis.popov@gmail.com	1988-11-05	Новый посетитель	4514	345678	/photos/visitors/popov.jpg	/scans/visitors/popov_passport.pdf	ООО "ЛогистикЦентр"	2	2	hash_visitor_3_pass	visitor_3
4	Васильева	Екатерина	Сергеевна	+7(916)456-78-90	vasilyeva@bk.ru	1992-09-28	Представитель партнерской организации	4515	456789	/photos/visitors/vasilyeva.jpg	/scans/visitors/vasilyeva_passport.pdf	АО "ЭнергоПром"	8	7	hash_visitor_4_pass	visitor_4
5	Михайлов	Артем	Владимирович	+7(916)567-89-01	artem.mihaylov@company.ru	1985-12-12	Кандидат на должность	4516	567890	/photos/visitors/mikhailov.jpg	/scans/visitors/mikhailov_passport.pdf	\N	4	3	hash_visitor_5_pass	visitor_5
6	Федорова	Ольга	Ивановна	+7(916)678-90-12	olga.fedorova@mail.ru	1990-04-17	Аудитор	4517	678901	/photos/visitors/fedorova.jpg	/scans/visitors/fedorova_passport.pdf	ООО "АудитПро"	5	4	hash_visitor_6_pass	visitor_6
7	Морозов	Игорь	Александрович	+7(916)789-01-23	morozov.igor@yandex.ru	1983-08-09	Технический специалист	4518	789012	/photos/visitors/morozov.jpg	/scans/visitors/morozov_passport.pdf	ООО "ТехноСервис"	3	2	hash_visitor_7_pass	visitor_7
8	Алексеева	Наталья	Дмитриевна	+7(916)890-12-34	alekseeva.n@bk.ru	1998-01-25	Стажер	4519	890123	/photos/visitors/alekseeva.jpg	/scans/visitors/alekseeva_passport.pdf	\N	2	2	hash_visitor_8_pass	visitor_8
9	Егоров	Павел	Николаевич	+7(916)901-23-45	egorov.pavel@gmail.com	1987-06-30	Поставщик оборудования	4520	901234	/photos/visitors/egorov.jpg	/scans/visitors/egorov_passport.pdf	ООО "КомплектСервис"	7	6	hash_visitor_9_pass	visitor_9
10	Николаева	Светлана	Валерьевна	+7(916)012-34-56	svetlana.n@company.ru	1991-10-19	Представитель клиента	4521	012345	/photos/visitors/nikolaeva.jpg	/scans/visitors/nikolaeva_passport.pdf	АО "КлиентПлюс"	1	1	hash_visitor_10_pass	visitor_10
11	Смирнов	Олег	Викторович	+7(916)123-45-67	oleg.smirnov@mail.ru	1980-03-21	Постоянный посетитель	4512	123456	/photos/visitors/smirnov.jpg	/scans/visitors/smirnov_passport.pdf	ООО "ТехноСервис"	3	2	hash_visitor_11_pass	visitor_11
12	Кузнецова	Мария	Петровна	+7(916)234-56-78	maria.kuznetsova@yandex.ru	1995-07-14	\N	4513	234567	/photos/visitors/kuznetsova.jpg	/scans/visitors/kuznetsova_passport.pdf	ЗАО "СтройИнвест"	4	3	hash_visitor_12_pass	visitor_12
13	Попов	Денис	Андреевич	+7(916)345-67-89	denis.popov@gmail.com	1988-11-05	Новый посетитель	4514	345678	/photos/visitors/popov.jpg	/scans/visitors/popov_passport.pdf	ООО "ЛогистикЦентр"	2	2	hash_visitor_13_pass	visitor_13
14	Васильева	Екатерина	Сергеевна	+7(916)456-78-90	vasilyeva@bk.ru	1992-09-28	Представитель партнерской организации	4515	456789	/photos/visitors/vasilyeva.jpg	/scans/visitors/vasilyeva_passport.pdf	АО "ЭнергоПром"	8	7	hash_visitor_14_pass	visitor_14
15	Михайлов	Артем	Владимирович	+7(916)567-89-01	artem.mihaylov@company.ru	1985-12-12	Кандидат на должность	4516	567890	/photos/visitors/mikhailov.jpg	/scans/visitors/mikhailov_passport.pdf	\N	4	3	hash_visitor_15_pass	visitor_15
16	Федорова	Ольга	Ивановна	+7(916)678-90-12	olga.fedorova@mail.ru	1990-04-17	Аудитор	4517	678901	/photos/visitors/fedorova.jpg	/scans/visitors/fedorova_passport.pdf	ООО "АудитПро"	5	4	hash_visitor_16_pass	visitor_16
17	Морозов	Игорь	Александрович	+7(916)789-01-23	morozov.igor@yandex.ru	1983-08-09	Технический специалист	4518	789012	/photos/visitors/morozov.jpg	/scans/visitors/morozov_passport.pdf	ООО "ТехноСервис"	3	2	hash_visitor_17_pass	visitor_17
18	Алексеева	Наталья	Дмитриевна	+7(916)890-12-34	alekseeva.n@bk.ru	1998-01-25	Стажер	4519	890123	/photos/visitors/alekseeva.jpg	/scans/visitors/alekseeva_passport.pdf	\N	2	2	hash_visitor_18_pass	visitor_18
19	Егоров	Павел	Николаевич	+7(916)901-23-45	egorov.pavel@gmail.com	1987-06-30	Поставщик оборудования	4520	901234	/photos/visitors/egorov.jpg	/scans/visitors/egorov_passport.pdf	ООО "КомплектСервис"	7	6	hash_visitor_19_pass	visitor_19
20	Николаева	Светлана	Валерьевна	+7(916)012-34-56	svetlana.n@company.ru	1991-10-19	Представитель клиента	4521	012345	/photos/visitors/nikolaeva.jpg	/scans/visitors/nikolaeva_passport.pdf	АО "КлиентПлюс"	1	1	hash_visitor_20_pass	visitor_20
21	Смирнов	Олег	Викторович	+7(916)123-45-67	oleg.smirnov@mail.ru	1980-03-21	Постоянный посетитель	4512	123456	/photos/visitors/smirnov.jpg	/scans/visitors/smirnov_passport.pdf	ООО "ТехноСервис"	3	2	hash_visitor_21_pass	visitor_21
22	Кузнецова	Мария	Петровна	+7(916)234-56-78	maria.kuznetsova@yandex.ru	1995-07-14	\N	4513	234567	/photos/visitors/kuznetsova.jpg	/scans/visitors/kuznetsova_passport.pdf	ЗАО "СтройИнвест"	4	3	hash_visitor_22_pass	visitor_22
23	Попов	Денис	Андреевич	+7(916)345-67-89	denis.popov@gmail.com	1988-11-05	Новый посетитель	4514	345678	/photos/visitors/popov.jpg	/scans/visitors/popov_passport.pdf	ООО "ЛогистикЦентр"	2	2	hash_visitor_23_pass	visitor_23
24	Васильева	Екатерина	Сергеевна	+7(916)456-78-90	vasilyeva@bk.ru	1992-09-28	Представитель партнерской организации	4515	456789	/photos/visitors/vasilyeva.jpg	/scans/visitors/vasilyeva_passport.pdf	АО "ЭнергоПром"	8	7	hash_visitor_24_pass	visitor_24
25	Михайлов	Артем	Владимирович	+7(916)567-89-01	artem.mihaylov@company.ru	1985-12-12	Кандидат на должность	4516	567890	/photos/visitors/mikhailov.jpg	/scans/visitors/mikhailov_passport.pdf	\N	4	3	hash_visitor_25_pass	visitor_25
1	Смирнов	Олег	Викторович	+7(916)123-45-67	oleg.smirnov@mail.ru	1980-03-21	Постоянный посетитель	4512	123456	/photos/visitors/smirnov.jpg	/scans/visitors/smirnov_passport.pdf	ООО "ТехноСервис"	3	2	e10adc3949ba59abbe56e057f20f883e	ts
26	Федорова	Ольга	Ивановна	+7(916)678-90-12	olga.fedorova@mail.ru	1990-04-17	Аудитор	4517	678901	/photos/visitors/fedorova.jpg	/scans/visitors/fedorova_passport.pdf	ООО "АудитПро"	5	4	hash_visitor_26_pass	visitor_26
27	Морозов	Игорь	Александрович	+7(916)789-01-23	morozov.igor@yandex.ru	1983-08-09	Технический специалист	4518	789012	/photos/visitors/morozov.jpg	/scans/visitors/morozov_passport.pdf	ООО "ТехноСервис"	3	2	hash_visitor_27_pass	visitor_27
28	Алексеева	Наталья	Дмитриевна	+7(916)890-12-34	alekseeva.n@bk.ru	1998-01-25	Стажер	4519	890123	/photos/visitors/alekseeva.jpg	/scans/visitors/alekseeva_passport.pdf	\N	2	2	hash_visitor_28_pass	visitor_28
29	Егоров	Павел	Николаевич	+7(916)901-23-45	egorov.pavel@gmail.com	1987-06-30	Поставщик оборудования	4520	901234	/photos/visitors/egorov.jpg	/scans/visitors/egorov_passport.pdf	ООО "КомплектСервис"	7	6	hash_visitor_29_pass	visitor_29
30	Николаева	Светлана	Валерьевна	+7(916)012-34-56	svetlana.n@company.ru	1991-10-19	Представитель клиента	4521	012345	/photos/visitors/nikolaeva.jpg	/scans/visitors/nikolaeva_passport.pdf	АО "КлиентПлюс"	1	1	hash_visitor_30_pass	visitor_30
31	dfgdfg	dfgdfg	dfgdfg	2344357634	artemskokov264@gmail.com	2006-04-18		4353	345345	\N	\N		\N	\N	e10adc3949ba59abbe56e057f20f883e	artemskokov264_32
33	asdasd	asdasd	asdasd	1232134234	arozonarp1092@gmail.com	2006-04-20		2133	123123	\N	\N		\N	\N	d41d8cd98f00b204e9800998ecf8427e	arozonarp1092_34
35	Смирнов	Олег	Викторович	+7(916)123-45-67	oleg@mail.ru	1980-03-21	\N	4512	123456	\N	\N	ООО ТехноСервис	\N	\N	482c811da5d5b4bc6d497ffa98491e38	oleg_36
37	Кузнецова	Мария	Петровна	+7(916)234-56-78	maria@mail.ru	1995-07-14	\N	4513	234567	\N	\N	ЗАО СтройИнвест	\N	\N	482c811da5d5b4bc6d497ffa98491e38	maria_38
39	Попов	Денис	Андреевич	+7(916)345-67-89	denis@mail.ru	1988-11-05	\N	4514	345678	\N	\N	ООО ЛогистикЦентр	\N	\N	482c811da5d5b4bc6d497ffa98491e38	denis_40
\.


--
-- TOC entry 4959 (class 0 OID 16752)
-- Dependencies: 222
-- Data for Name: Сотрудник; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Сотрудник" (id, "Фамилия", "Имя", "Отчество", "Телефон", email, "Дата_рождения", "Примечание", "Логин", "Пароль", "Серия_паспорта", "Номер_паспорта", "Фото_Посетителя", "скан_паспорта_посетителя", "Подразделение_id") FROM stdin;
2	Петрова	Анна	Владимировна	+7(999)234-56-78	petrova@company.ru	1990-08-22	Ведущий специалист	petrova.a	hash_password_2	4513	456789	/photos/petrova.jpg	/scans/petrova_passport.pdf	2
3	Сидоров	Михаил	Алексеевич	+7(999)345-67-89	sidorov@company.ru	1988-03-10	Старший инженер	sidorov.m	hash_password_3	4514	567890	/photos/sidorov.jpg	/scans/sidorov_passport.pdf	2
4	Козлова	Елена	Дмитриевна	+7(999)456-78-90	kozlova@company.ru	1992-11-30	Менеджер по персоналу	kozlova.e	hash_password_4	4515	678901	/photos/kozlova.jpg	/scans/kozlova_passport.pdf	3
5	Соколов	Дмитрий	Игоревич	+7(999)567-89-01	sokolov@company.ru	1983-07-19	Главный бухгалтер	sokolov.d	hash_password_5	4516	789012	/photos/sokolov.jpg	/scans/sokolov_passport.pdf	4
6	Морозова	Татьяна	Николаевна	+7(999)678-90-12	morozova@company.ru	1995-04-25	Администратор	morozova.t	hash_password_6	4517	890123	/photos/morozova.jpg	/scans/morozova_passport.pdf	5
7	Волков	Андрей	Павлович	+7(999)789-01-23	volkov@company.ru	1987-09-14	Начальник отдела пропусков	volkov.a	hash_password_7	4518	901234	/photos/volkov.jpg	/scans/volkov_passport.pdf	6
8	Новикова	Ирина	Александровна	+7(999)890-12-34	novikova@company.ru	1993-12-01	Юрисконсульт	novikova.i	hash_password_8	4519	012345	/photos/novikova.jpg	/scans/novikova_passport.pdf	7
9	Зайцев	Алексей	Романович	+7(999)901-23-45	zaytsev@company.ru	1989-06-18	Инженер по охране труда	zaytsev.a	hash_password_9	4520	123456	/photos/zaytsev.jpg	/scans/zaytsev_passport.pdf	8
1	Иванов	Петр	Сергеевич	+7(999)123-45-67	ivanov@company.ru	1985-05-15	Начальник отдела	tsso	e10adc3949ba59abbe56e057f20f883e	4512	345678	/photos/ivanov.jpg	/scans/ivanov_passport.pdf	1
10	Сидоров	Иван	\N	\N	sidorov@security.ru	\N	\N	security_02	83af648e6d9712795f2cb32ad6c77592	\N	\N	\N	\N	6
\.


--
-- TOC entry 4965 (class 0 OID 16848)
-- Dependencies: 228
-- Data for Name: Черный_список; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Черный_список" (id, "Посетитель_id", "Причина", "Дата_добавления") FROM stdin;
1	1	Нарушение правил посещения объекта КИИ	2026-04-18 10:06:40.467731
\.


--
-- TOC entry 4978 (class 0 OID 0)
-- Dependencies: 225
-- Name: Заявка_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Заявка_id_seq"', 30, true);


--
-- TOC entry 4979 (class 0 OID 0)
-- Dependencies: 219
-- Name: Подразделение_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Подразделение_id_seq"', 1, false);


--
-- TOC entry 4980 (class 0 OID 0)
-- Dependencies: 223
-- Name: Посетитель_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Посетитель_id_seq"', 40, true);


--
-- TOC entry 4981 (class 0 OID 0)
-- Dependencies: 221
-- Name: Сотрудник_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Сотрудник_id_seq"', 11, true);


--
-- TOC entry 4982 (class 0 OID 0)
-- Dependencies: 227
-- Name: Черный_список_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Черный_список_id_seq"', 1, true);


--
-- TOC entry 4797 (class 2606 OID 16810)
-- Name: Заявка Заявка_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Заявка"
    ADD CONSTRAINT "Заявка_pkey" PRIMARY KEY (id);


--
-- TOC entry 4787 (class 2606 OID 16750)
-- Name: Подразделение Подразделение_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Подразделение"
    ADD CONSTRAINT "Подразделение_pkey" PRIMARY KEY (id);


--
-- TOC entry 4793 (class 2606 OID 16784)
-- Name: Посетитель Посетитель_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Посетитель"
    ADD CONSTRAINT "Посетитель_pkey" PRIMARY KEY (id);


--
-- TOC entry 4795 (class 2606 OID 16836)
-- Name: Посетитель Посетитель_Логин_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Посетитель"
    ADD CONSTRAINT "Посетитель_Логин_key" UNIQUE ("Логин");


--
-- TOC entry 4789 (class 2606 OID 16765)
-- Name: Сотрудник Сотрудник_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Сотрудник"
    ADD CONSTRAINT "Сотрудник_pkey" PRIMARY KEY (id);


--
-- TOC entry 4791 (class 2606 OID 16767)
-- Name: Сотрудник Сотрудник_Логин_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Сотрудник"
    ADD CONSTRAINT "Сотрудник_Логин_key" UNIQUE ("Логин");


--
-- TOC entry 4800 (class 2606 OID 16859)
-- Name: Черный_список Черный_список_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Черный_список"
    ADD CONSTRAINT "Черный_список_pkey" PRIMARY KEY (id);


--
-- TOC entry 4798 (class 1259 OID 16865)
-- Name: idx_blacklist_visitor; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_blacklist_visitor ON public."Черный_список" USING btree ("Посетитель_id");


--
-- TOC entry 4808 (class 2620 OID 16846)
-- Name: Посетитель visitor_insert_trigger; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER visitor_insert_trigger BEFORE INSERT ON public."Посетитель" FOR EACH ROW EXECUTE FUNCTION public.generate_visitor_login();


--
-- TOC entry 4807 (class 2606 OID 16860)
-- Name: Черный_список fk_blacklist_visitor; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Черный_список"
    ADD CONSTRAINT fk_blacklist_visitor FOREIGN KEY ("Посетитель_id") REFERENCES public."Посетитель"(id) ON DELETE CASCADE;


--
-- TOC entry 4804 (class 2606 OID 16821)
-- Name: Заявка Заявка_Подразделение_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Заявка"
    ADD CONSTRAINT "Заявка_Подразделение_id_fkey" FOREIGN KEY ("Подразделение_id") REFERENCES public."Подразделение"(id) ON DELETE SET NULL;


--
-- TOC entry 4805 (class 2606 OID 16811)
-- Name: Заявка Заявка_Посетитель_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Заявка"
    ADD CONSTRAINT "Заявка_Посетитель_id_fkey" FOREIGN KEY ("Посетитель_id") REFERENCES public."Посетитель"(id) ON DELETE CASCADE;


--
-- TOC entry 4806 (class 2606 OID 16816)
-- Name: Заявка Заявка_Сотрудник_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Заявка"
    ADD CONSTRAINT "Заявка_Сотрудник_id_fkey" FOREIGN KEY ("Сотрудник_id") REFERENCES public."Сотрудник"(id) ON DELETE SET NULL;


--
-- TOC entry 4802 (class 2606 OID 16790)
-- Name: Посетитель Посетитель_Подразделение_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Посетитель"
    ADD CONSTRAINT "Посетитель_Подразделение_id_fkey" FOREIGN KEY ("Подразделение_id") REFERENCES public."Подразделение"(id) ON DELETE SET NULL;


--
-- TOC entry 4803 (class 2606 OID 16785)
-- Name: Посетитель Посетитель_Сотрудник_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Посетитель"
    ADD CONSTRAINT "Посетитель_Сотрудник_id_fkey" FOREIGN KEY ("Сотрудник_id") REFERENCES public."Сотрудник"(id) ON DELETE SET NULL;


--
-- TOC entry 4801 (class 2606 OID 16768)
-- Name: Сотрудник Сотрудник_Подразделение_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Сотрудник"
    ADD CONSTRAINT "Сотрудник_Подразделение_id_fkey" FOREIGN KEY ("Подразделение_id") REFERENCES public."Подразделение"(id) ON DELETE SET NULL;


-- Completed on 2026-04-21 08:17:11

--
-- PostgreSQL database dump complete
--

\unrestrict fTGEvaxh9eLf9W9B1docXHyyVpH5BhITjpAlvgeVEBrDqyMwrjh4R759DDeEWUT

-- Completed on 2026-04-21 08:17:11

--
-- PostgreSQL database cluster dump complete
--

