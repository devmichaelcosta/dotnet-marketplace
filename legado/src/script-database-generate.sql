-- MySQL dump 10.13  Distrib 8.0.12, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: examplemarketplace
-- ------------------------------------------------------
-- Server version	8.0.12

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `addresses`
--

DROP TABLE IF EXISTS `addresses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `addresses` (
  `AddressId` int(11) NOT NULL AUTO_INCREMENT,
  `Street` varchar(255) NOT NULL,
  `CEP` varchar(9) NOT NULL,
  `Neighborhood` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `City` varchar(100) NOT NULL,
  `StateId` int(11) NOT NULL,
  `Complement` varchar(255) NOT NULL,
  `UserId` int(11) NOT NULL,
  PRIMARY KEY (`AddressId`),
  KEY `StateId` (`StateId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `addresses_ibfk_1` FOREIGN KEY (`StateId`) REFERENCES `states` (`stateid`),
  CONSTRAINT `addresses_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `addresses`
--

LOCK TABLES `addresses` WRITE;
/*!40000 ALTER TABLE `addresses` DISABLE KEYS */;
INSERT INTO `addresses` VALUES (1,'Rua Marechal Floriano, 1001 - ','43123-421','Centro','Caxias do sul',1,'',5);
/*!40000 ALTER TABLE `addresses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `attributes`
--

DROP TABLE IF EXISTS `attributes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `attributes` (
  `AttributeId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  PRIMARY KEY (`AttributeId`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `attributes`
--

LOCK TABLES `attributes` WRITE;
/*!40000 ALTER TABLE `attributes` DISABLE KEYS */;
INSERT INTO `attributes` VALUES (1,'Peso'),(2,'Largura'),(3,'Altura'),(4,'Número de páginas'),(5,'Editora'),(7,'Nome'),(8,'Resolução'),(9,'Tamanho'),(10,'Marca'),(11,'Modelo'),(12,'Cor'),(13,'Corpo'),(14,'Braço'),(15,'Escala'),(16,'Tensor'),(17,'Trastes'),(18,'Marcações'),(19,'Tarraxas'),(20,'Nut'),(21,'Ponte'),(22,'Captador'),(23,'Controles'),(24,'Acabamento'),(25,'Acompanhamento'),(26,'Valor energético'),(27,'Carboidratos'),(28,'Proteínas'),(29,'Gorduras totais'),(30,'Gorduras saturadas	'),(31,'Gorduras trans');
/*!40000 ALTER TABLE `attributes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `attributevalues`
--

DROP TABLE IF EXISTS `attributevalues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `attributevalues` (
  `AttributeValueId` int(11) NOT NULL AUTO_INCREMENT,
  `AttributeId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL,
  `Value` varchar(255) NOT NULL,
  PRIMARY KEY (`AttributeValueId`),
  KEY `AtributeId` (`AttributeId`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `attributevalues_ibfk_1` FOREIGN KEY (`AttributeId`) REFERENCES `attributes` (`AttributeId`),
  CONSTRAINT `attributevalues_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `products` (`productid`)
) ENGINE=InnoDB AUTO_INCREMENT=102 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `attributevalues`
--

LOCK TABLES `attributevalues` WRITE;
/*!40000 ALTER TABLE `attributevalues` DISABLE KEYS */;
INSERT INTO `attributevalues` VALUES (1,1,11,'aaaa'),(2,1,11,'aaaa'),(38,1,15,'fsafda'),(39,8,15,'sdafd'),(74,24,16,'Envernizado'),(75,25,16,'Acompanha bag SX BB400'),(76,14,16,'Maple Canadense selecionado com reforço em Rosewood'),(77,22,16,'Single Coil PB'),(78,23,16,'01 Volume e 01 Tone'),(79,12,16,'Preto'),(80,13,16,'Basswood sólido'),(81,15,16,'Rosewood com 862mm de comprimento'),(82,10,16,'SX'),(83,18,16,'Pontos'),(84,11,16,'SPB62+'),(85,20,16,'Osso sintético de 42mm'),(86,21,16,'Cromada'),(87,19,16,'Cromadas estilo vintage'),(88,16,16,'ação dupla'),(89,17,16,'20 em níquel'),(90,8,10,'3840x1200'),(91,7,10,'fdsadfasf'),(92,27,14,'3g'),(93,30,14,'0g'),(94,29,14,'0g'),(95,31,14,'0g'),(96,28,14,'24g'),(97,26,14,'122,4kcal = 512KJ'),(98,14,17,'Natowood'),(99,12,17,'Azur'),(100,13,17,'Sapele'),(101,15,17,'Escala');
/*!40000 ALTER TABLE `attributevalues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `carouselimages`
--

DROP TABLE IF EXISTS `carouselimages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `carouselimages` (
  `CarouselImageId` int(11) NOT NULL AUTO_INCREMENT,
  `FileName` varchar(255) NOT NULL,
  `Order` int(11) NOT NULL,
  PRIMARY KEY (`CarouselImageId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `carouselimages`
--

LOCK TABLES `carouselimages` WRITE;
/*!40000 ALTER TABLE `carouselimages` DISABLE KEYS */;
INSERT INTO `carouselimages` VALUES (1,'guitar-1920x384-1.jpg',1),(2,'guitar-1920x384-2.jpg',2),(3,'guitar-1920x384-3.jpg',3);
/*!40000 ALTER TABLE `carouselimages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `categories` (
  `CategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `Image` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`CategoryId`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
INSERT INTO `categories` VALUES (4,'Ferramentas','8bb64aa0184f8d6138a262b5926db6ae.jpg'),(5,'Instrumentos musicais','5d3c5b7a93218_gg.jpg'),(6,'Livros','51d1qVhmAmL.jpg'),(12,'Veículos','teste0.jpg'),(13,'Suplementos','whey-protein.png'),(15,'Teste 123','');
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orderitens`
--

DROP TABLE IF EXISTS `orderitens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `orderitens` (
  `OrderItemId` int(11) NOT NULL AUTO_INCREMENT,
  `OrderId` int(11) NOT NULL,
  `ProductId` int(11) DEFAULT NULL,
  `Qtd` int(11) DEFAULT NULL,
  PRIMARY KEY (`OrderItemId`),
  KEY `OrderId` (`OrderId`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `orderitens_ibfk_1` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`orderid`),
  CONSTRAINT `orderitens_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `products` (`productid`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orderitens`
--

LOCK TABLES `orderitens` WRITE;
/*!40000 ALTER TABLE `orderitens` DISABLE KEYS */;
INSERT INTO `orderitens` VALUES (1,1,1,3),(2,2,7,1),(3,3,10,1),(4,4,6,1);
/*!40000 ALTER TABLE `orderitens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `orders` (
  `OrderId` int(11) NOT NULL AUTO_INCREMENT,
  `Total` int(11) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `UserId` int(11) NOT NULL,
  `StateId` int(11) NOT NULL,
  `CardOwnerName` varchar(150) NOT NULL,
  `ExpirationDate` varchar(150) NOT NULL,
  `Name` varchar(150) NOT NULL,
  `Address` varchar(150) NOT NULL,
  `Neighborhood` varchar(150) NOT NULL,
  `CPF` varchar(14) NOT NULL,
  `CEP` varchar(9) NOT NULL,
  `City` varchar(150) NOT NULL,
  `Complement` varchar(150) NOT NULL,
  PRIMARY KEY (`OrderId`),
  KEY `UserId` (`UserId`),
  KEY `StateId` (`StateId`),
  CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`userid`),
  CONSTRAINT `orders_ibfk_2` FOREIGN KEY (`StateId`) REFERENCES `states` (`stateid`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,17073,'2020-08-24 12:34:55',1,1,'4312431234','1431234341','ffdasfasfds','432134124','43124314321','014.194.750-02','43214-312','4312431234','43214324'),(2,78,'2020-11-29 17:42:37',3,1,'Tatiana','123123','Tatiana','fdasfdsadfsafdsafasdf','Diamantino','121.313.221-33','99999-999','Caxias do sul','fdasfdasfdsa'),(3,92,'2020-12-05 13:30:25',1,1,'Michael','1234564789789789789789','Michael','Rua Whatever','Whatever','123.123.123-12','12312-312','Caxias do sul','Não interessa'),(4,990,'2020-12-07 17:08:14',8,1,'Tatiana','9999','Tatiana','Rua 123456','Bairro 123456','141.243.141-24','95055-041','Caxias do sul','LALALALLALALA');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `products` (
  `ProductId` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `Description` varchar(10000) DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` varchar(255) DEFAULT NULL,
  `Offer` bit(1) NOT NULL,
  `Stock` int(11) NOT NULL,
  `Sku` varchar(45) NOT NULL,
  `UserId` int(11) NOT NULL,
  `SubCategoryId` int(11) DEFAULT NULL,
  PRIMARY KEY (`ProductId`),
  KEY `UserId` (`UserId`),
  KEY `SubCategoryId` (`SubCategoryId`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`userid`),
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`SubCategoryId`) REFERENCES `subcategories` (`subcategoryid`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'GUITARRA FENDER STANDARD TELECASTER MEXICANA BLACK - 014-5102-506','A Fender traz a Standard Telecaster para guitarristas que apreciam estilo e versatilidade por um super valor!',5690.99,'2020-07-31 23:13:06','michael',_binary '',7,'001',2,1),(2,'GUITARRA FENDER AMERICAN SPECIAL STRATOCASTER MAPLE 2-COLOR SUNBURST (2012) - ACOMPANHA HARD CASE','A lendária guitarra Stratocaster em sua versão mais tradicional!',7990.00,'2020-07-31 23:13:06','michael',_binary '',10,'002',2,1),(3,'GUITARRA JACKSON DINKY JS11 GLOSS BLACK - 291 0110 503','Uma guitarra de alta qualidade e excelente preço da lendária marca Jackson!',1790.00,'2020-07-31 23:13:06','michael',_binary '',10,'003',2,1),(6,'GUITARRA TAGIMA TW SERIES TG-530 MR STRATOCASTER METALLIC RED','Inspirada nos clássicos dos anos 60 e 70, a Tagima criou esta nova série de guitarras magníficas com ótimo custo benefício!',990.00,'2020-11-20 21:08:27','michael',_binary '',1,'001',5,1),(7,'Domain-driven Design - Atacando As Complexidades No Coração Do Software - 3ª Edição - 3ª Ed.','A comunidade de desenvolvimento de softwares reconhece que a modelagem de domínios é fundamental para o design de softwares. Através de modelos de domínios, os desenvolvedores de software conseguem expressar valiosas funcionalidades e traduzi-las em uma implementação de software que realmente atenda às necessidades de seus usuários. Mas, apesar de sua óbvia importância, existem poucos recursos práticos que explicam como incorporar uma modelagem de domínios eficiente no processo de desenvolvimento de softwares.\r\n\r\nO Domain-Driven Design atende essa necessidade. Este não é um livro sobre tecnologias específicas. Ele oferece aos leitores uma abordagem sistemática com relação ao domain-driven design, ou DDD, apresentando um conjunto abrangente de práticas ideais de design, técnicas baseadas em experiências e princípios fundamentais que facilitam o desenvolvimento de projetos de software que enfrentam domínios complexos. Reunindo práticas de design e implementação, este livro incorpora vários exemplos baseados em projetos que ilustram a aplicação do design dirigido por domínios no desenvolvimento de softwares na vida real.\r\n\r\nCom este livro em mãos, desenvolvedores orientados a objetos, analistas de sistema e designers terão a orientação de que precisam para organizar e concentrar seu trabalho, criar modelos de domínio valiosos e úteis, e transformar esses modelos em implementações de software duradouras e de alta qualidade.',77.70,'2020-11-22 14:07:54','michael',_binary '',11,'130779534',7,NULL),(8,'Arquitetura Limpa: O guia do artesão para estrutura e design de software','As regras universais de arquitetura de software aumentam dramaticamente a produtividade dos desenvolvedores ao longo da vida dos sistemas de software. Agora, aproveitando o sucesso dos seus best-sellers Código Limpo e O Codificador Limpo, o lendário artesão de software Robert C. Martin (&#34;Uncle Bob&#34;) vai revelar essas regras e ajudar o leitor a aplicá-las.\r\nA Arquitetura Limpa de Martin não é só mais um catálogo de opções. Com base em meio século de experiência nos mais variados ambientes de software, Martin indica as escolhas que você deve fazer e explica por que elas são cruciais para o seu sucesso. Como já era esperado do Uncle Bob, este livro está cheio de soluções simples e diretas para os desafios reais que você enfrentará — aqueles que irão influenciar diretamente o sucesso ou fracasso dos seus projetos.\r\n\r\n- Aprenda sobre as metas dos arquitetos de software — e as principais disciplinas e práticas que podem concretizá-las;\r\n- Domine os princípios essenciais do design de software para abordar função, separação de componentes e gestão de dados;\r\n- Veja como os paradigmas de programação impõem disciplina ao restringirem as ações dos desenvolvedores;\r\n- Saiba identificar o que é crucialmente importante e o que é apenas um &#34;detalhe&#34;;\r\n- Implemente estruturas ótimas e de alto nível para web, banco de dados, thick-client, console e aplicativos incorporados;\r\n- Defina limites e camadas adequadas e organize os componentes e serviços;\r\n- Saiba por que designs e arquiteturas dão errado e como prevenir (ou corrigir) essas falhas;\r\n\r\nArquitetura Limpa é uma leitura essencial para profissionais que já atuam ou querem ingressar no mercado, como arquitetos de software, analistas de sistemas, designers de sistemas, gerentes de software e programadores que precisam executar designs de outras pessoas.',66.33,'2020-11-22 14:20:14','michael',_binary '',12,'4231434',4,4),(9,'O Codificador Limpo ','Então você quer ser um profissional do desenvolvimento de softwares. Quer erguer a cabeça e declarar para o mundo: “Eu sou um profissional!”.\r\nQuer que as pessoas olhem para você com respeito e o tratem com consideração. Você quer isso tudo.\r\nCerto?\r\n\r\nO termo “Profissionalismo” é, sem dúvida, um distintivo de honra e orgulho, mas também é um marcador de incumbência e responsabilidade, que inclui trabalhar bem e honestamente.\r\n\r\nVerdadeiros profissionais praticam e trabalham firme para manter suas habilidades afiadas e prontas.\r\nNão é o bastante simplesmente fazer suas tarefas diárias e chamar isso de prática.\r\nRealizar seu trabalho diário é performance, e não prática. Prática é quando você especificamente exercita as habilidades fora do seu ambiente de trabalho com o único propósito de potencializá-las.\r\n\r\nO Codificador Limpo contém muitos conselhos pragmáticos que visam transformar o comportamento do profissional de software.\r\nO autor transmite valiosos ensinamentos sobre ética, respeito, responsabilidade, sinceridade e comprometimento, através de sua experiência como programador.',48.90,'2020-11-22 14:21:16','michael',_binary '',2,'43124312',4,NULL),(10,'Código Limpo - Habilidades Praticas do Agile Software','Mesmo um código ruim pode funcionar. Mas se ele não for limpo, pode acabar com uma empresa de desenvolvimento. Perdem-se a cada ano horas incontáveis e recursos importantes devido a um código mal escrito. Mas não precisa ser assim. O renomado especialista em software, Robert C. Martin, apresenta um paradigma revolucionário com Código limpo: Habilidades Práticas do Agile Software. Martin se reuniou com seus colegas do Mentor Object para destilar suas melhores e mais ágeis práticas de limpar códigos “dinamicamente” em um livro que introduzirá gradualmente dentro de você os valores da habilidade de um profissional de softwares e lhe tornar um programador melhor –mas só se você praticar. Que tipo de trabalho você fará Você lerá códigos aqui, muitos códigos. E você deverá descobrir o que está correto e errado nos códigos. E, o mais importante, você terá de reavaliar seus valores profissionais e seu comprometimento com o seu ofício. Código limpo está divido em três partes. Na primeira há diversos capítulos que descrevem os princípios, padrões e práticas para criar um código limpo. A segunda parte consiste em diversos casos de estudo de complexidade cada vez maior. Cada um é um exercício para limpar um código – transformar o código base que possui alguns problemas em um melhor e eficiente. A terceira parte é a compensação: um único capítulo com uma lista de heurísticas e “odores” reunidos durante a criação dos estudos de caso. O resultado será um conhecimento base que descreve a forma como pensamos quando criamos, lemos e limpamos um código. Após ler este livro os leitores saberão: Como distinguir um código bom de um ruim Como escrever códigos bons e como transformar um ruim em um bom Como criar bons nomes, boas funções, bons objetos e boas classes Como formatar o código para ter uma legibilidade máxima Como implementar completamente o tratamento de erro sem obscurecer a lógica Como aplicar testes de unidade e praticar o desenvolvimento dirigido a testes',91.93,'2020-11-22 14:22:29','michael',_binary '',1,'414236754',4,4),(11,'Princípios, Padrões e Práticas Ágeis em C#','Destinado a programadores, gerentes de desenvolvimento de software e analistas de negócios, contém diversos estudos de caso que ilustram os fundamentos do desenvolvimento e do projeto ágil, e mostram a evolução de modelos UML em códigos C# reais. Esta obra fornece uma visão completa sobre os princípios ágeis e as práticas da Programação Extrema; o desenvolvimento baseado em testes; a refatoração com testes de unidade; a programação em pares; e muito mais.',275.00,'2020-11-22 14:29:10','michael',_binary '',2,'9788577808427',4,NULL),(12,'Clean Agile: Back to Basics','“In the journey to all things Agile, Uncle Bob has been there, done that, and has the both the t-shirt and the scars to show for it. This delightful book is part history, part personal stories, and all wisdom. If you want to understand what Agile is and how it came to be, this is the book for you.”\r\n–Grady Booch\r\n“Bob’s frustration colors every sentence of Clean Agile, but it’s a justified frustration. What is in the world of Agile development is nothing compared to what could be. This book is Bob’s perspective on what to focus on to get to that ‘what could be.’ And he’s been there, so it’s worth listening.”\r\n–Kent Beck\r\n“It’s good to read Uncle Bob’s take on Agile. Whether just beginning, or a seasoned Agilista, you would do well to read this book. I agree with almost all of it. It’s just some of the parts make me realize my own shortcomings, dammit. It made me double-check our code coverage (85.09%).”\r\n–Jon Kern Nearly twenty years after the Agile Manifesto was first presented, the legendary Robert C. Martin (“Uncle Bob”) reintroduces Agile values and principles for a new generation–programmers and nonprogrammers alike. Martin, author of Clean Code and other highly influential software development guides, was there at Agile’s founding. Now, in Clean Agile: Back to Basics, he strips away misunderstandings and distractions that over the years have made it harder to use Agile than was originally intended.\r\nMartin describes what Agile is in no uncertain terms: a small discipline that helps small teams manage small projects . . . with huge implications because every big project is comprised of many small projects. Drawing on his fifty years’ experience with projects of every conceivable type, he shows how Agile can help you bring true professionalism to software development.\r\nGet back to the basics–what Agile is, was, and should always be\r\nUnderstand the origins, and proper practice, of SCRUM\r\nMaster essential business-facing Agile practices, from small releases and acceptance tests to whole-team communication\r\nExplore Agile team members’ relationships with each other, and with their product\r\nRediscover indispensable Agile technical practices: TDD, refactoring, simple design, and pair programming\r\nUnderstand the central roles values and craftsmanship play in your Agile team’s success\r\nIf you want Agile’s true benefits, there are no shortcuts: You need to do Agile right. Clean Agile: Back to Basics will show you how, whether you’re a developer, tester, manager, project manager, or customer.',237.99,'2020-11-22 14:31:03','michael',_binary '',3,'86587568',4,4),(13,'Extreme Programming in Practice','Extreme Programming (XP) is a lightweight methodology that enables small teams of developers to achieve breakthrough productivity and software quality, even when faced with rapidly changing or unclear requirements. In this new book, top object-oriented consultants James Newkirk and Robert Martin walk through an entire XP project, chronicling the adoption of XP by a team that has never used it before. Along the way, they show how to overcome the obstacles facing XP adopters, and present realistic XP best practices virtually any development organization can benefit from. The case study in this book is real, driven by the needs of a real customer. The artifacts, code, user stories, and anecdotes are all real, drawn from videotaped meetings throughout the project&#39;s development process. The result: an exceptionally true-to-life narrative, complete with mistakes and false starts, and reflecting the ebb and flow of a real project. For organizations considering XP, this may be the most realistic and useful guide ever produced. For project managers, developers, software engineers, XP customers, and upper-level managers.',210.02,'2020-11-22 14:34:05','michael',_binary '',34,'236534737',4,NULL),(14,'WHEY PROTEIN CONCENTRADO (1KG) - GROWTH SUPPLEMENTS','Otimize seus resultados ingerindo a proteína ideal\r\nWhey Protein Growth é a proteína ideal para quem treina hipertrofia e quer ganhar massa muscular.\r\n\r\nIdeal porque é um suplemento de alto valor biológico, com grande concentração de proteínas e aminoácidos e também rico em Glutamina, BCAA e Leucina.',76.50,'2020-11-28 21:39:30','michael',_binary '\0',20,'22431',2,7),(15,'Notebook Dell Inspiron I13-7391-A30S 10ª Intel Core I7 8GB (Geforce MX250 com 2GB) 512GB SSD Windows 10 13,3&#34; - Prata','*Peso aproximado de 1 kg. Pode variar de acordo com a configuração do produto.\r\n\r\n**A interação com aplicativos de dispositivos móveis por meio de espelhamento está disponível somente em dispositivos Android. Não há garantia de que o aplicativo funcionará em todos os dispositivos com Android.\r\n\r\n***Os serviços de garantia em domicílio poderão ser fornecidos por terceiros. Técnicos serão deslocados, se necessário, após consulta telefônica. A visita do técnico poderá ocorrer a partir do próximo dia útil, dependendo da região geográfica do Usuário Final e da disponibilidade imediata das peças para o reparo. Para solicitar atendimento, ligue para 0800 970 3355 ou 40040108 (ligações feitas a partir de celular para as regiões metropolitanas de Porto Alegre, Rio de Janeiro e São Paulo).\r\n\r\n****Produtos vendidos e entregues pelo Varejo contam com 12 meses de licença McAfee LiveSafe pré instalada, podendo ser utilizada em ilimitados dispositivos, tais como PCs, tablets e smartphones, na mesma assinatura.\r\n\r\n*****A licença se destina a uso pessoal dos seus dispositivos com suporte durante a vigência da assinatura. Consulte os requisitos do sistema para obter mais detalhes. Alguns recursos exigem hardware e/ou configuração adicionais. Para saber se o seu dispositivo é compatível, acesse: www.mcafee.com/SystemRequirements (em inglês)',6800.00,'2020-11-28 21:47:21','michael',_binary '',2,'45889456',2,NULL),(16,'CONTRABAIXO SX SPB62+ PRECISION BASS DE 4 CORDAS PRETO - ACOMPANHA BAG','O clássico Precision Bass em toda sua glória e com excelente preço! O contrabaixo SX SPB62+ é construído com materiais de alto padrão, com corpo em Basswood sólido e corte Vintage, braço em Maple Canadense selecionado à mão com reforço em Rosewood e escala em Rosewood com 862mm de comprimento. Equipado com captador single-coil estilo PB, o SX SPB62+ produz timbre encorpado, com muito rugido, ideal para o rock n&#39; roll, blues e jazz. Além disso, o SPB62+ possui um belíssimo acabamento envernizado, ferragens cromadas e tarraxas também no estilo Vintage, destacando-se tanto no visual quanto no som. Acompanha gig bag exclusivo da SX.',2234.00,'2020-11-28 21:54:49','michael',_binary '\0',2,'4315235',5,2),(17,'Violão Tagima Dallas Tuner Elétrico Cordas de Aço e com Afinador','Supreenda-se com a beleza e qualidade do Violão Elétrico Tagima Dallas, um dos mais vendidos da marca Tagima! Este violão apresenta um ótimo custo benefício.\r\n\r\nO violão Tagima Dallas possui design moderno com cutaway para alcance das casas mais agudas e com desenho do braço mais confortável, contribuindo bastante na performance do músico.\r\n\r\nPossui todo seu bojo (tampo, lateral e fundo) construído em excelentes madeiras, e tarraxas cromadas blindadas que auxiliam muito na manutenção da afinação.\r\n\r\nNa parte elétrica ele vem equipado com captação tipo piezo e pré-amplificador Tagima TEQ-8 ativo com controles de volume, equalizador com 4 bandas e afinador digital integrado ao sistema.',935.91,'2020-11-30 00:03:50','michael',_binary '',12,'E1010023682',2,3),(30,'Guitarra San Dilmas','fasdsafdsa',4600.00,'2020-11-30 00:14:49','michael',_binary '\0',1,'1',5,1);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productsimages`
--

DROP TABLE IF EXISTS `productsimages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `productsimages` (
  `ProductImageId` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` int(11) NOT NULL,
  `FileName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`ProductImageId`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `productsimages_ibfk_1` FOREIGN KEY (`ProductId`) REFERENCES `products` (`productid`)
) ENGINE=InnoDB AUTO_INCREMENT=113 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productsimages`
--

LOCK TABLES `productsimages` WRITE;
/*!40000 ALTER TABLE `productsimages` DISABLE KEYS */;
INSERT INTO `productsimages` VALUES (11,7,'51tiNslGehL._SX323_BO1,204,203,200_.png'),(12,7,'919QdUWbG-L.jpg'),(17,9,'codificador-limpo.jpg'),(18,9,'codificador-limpo-2.jpg'),(22,11,'principios-padroes-e-praticas-ageis-em-charp.jpg'),(24,13,'extreming_programing_inpratice.jpg'),(84,15,'diablo3 set do pavor2.png'),(89,30,'s-l1600s.jpg'),(90,2,'fender-mex-black-014-5102-506.jpg'),(91,1,'fender-american-especial-stratocaster-maple2-color-sunburst2012.jpg'),(92,3,'jackson-dincky-JS11GLOSSBLACK2910110503.jpg'),(96,16,'SX_SPB62.jpg'),(97,16,'SX_SPB62_3.jpg'),(98,16,'SX_SPB62_2.jpg'),(99,16,'SX_SPB62_4.jpg'),(100,8,'clean-architecture.jpg'),(101,8,'clean-archteture-2.jpg'),(102,8,'clean-archteture-3.jpg'),(103,8,'clean-archteture-4.jpg'),(104,12,'clean_agile.jpg'),(105,10,'clean-code-1.jpg'),(106,10,'clean-code-2.jpg'),(107,10,'clean-code-back.jpg'),(108,14,'whey-protein.png'),(109,17,'1090123_violao-tagima-dallas-tuner-eletrico-cordas-de-aco-e-com-afinador-ms_z2_637371635447323128.jpg'),(110,6,'TG-530 MR-1.jpg'),(111,6,'TG-530 MR-2.jpg'),(112,6,'TG-530 MR-3.jpg');
/*!40000 ALTER TABLE `productsimages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productslikeds`
--

DROP TABLE IF EXISTS `productslikeds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `productslikeds` (
  `ProductLikedId` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  PRIMARY KEY (`ProductLikedId`),
  KEY `FK_productslikeds_productId` (`ProductId`),
  KEY `FK_productslikeds_UserId` (`UserId`),
  CONSTRAINT `FK_productslikeds_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`userid`),
  CONSTRAINT `FK_productslikeds_productId` FOREIGN KEY (`ProductId`) REFERENCES `products` (`productid`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productslikeds`
--

LOCK TABLES `productslikeds` WRITE;
/*!40000 ALTER TABLE `productslikeds` DISABLE KEYS */;
INSERT INTO `productslikeds` VALUES (21,1,1),(23,16,1),(24,10,1),(25,2,1),(28,16,8),(31,8,8);
/*!40000 ALTER TABLE `productslikeds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ratings`
--

DROP TABLE IF EXISTS `ratings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `ratings` (
  `RatingId` int(11) NOT NULL AUTO_INCREMENT,
  `ProductId` int(11) NOT NULL,
  `Title` varchar(30) NOT NULL,
  `Description` varchar(100) NOT NULL,
  `Recommended` bit(1) NOT NULL,
  `Rating` varchar(30) NOT NULL,
  `UserId` int(11) NOT NULL,
  `Approved` bit(1) NOT NULL,
  PRIMARY KEY (`RatingId`),
  KEY `ProductId` (`ProductId`),
  KEY `FK_productscomments_UserId` (`UserId`),
  CONSTRAINT `FK_productscomments_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`userid`),
  CONSTRAINT `ratings_ibfk_1` FOREIGN KEY (`ProductId`) REFERENCES `products` (`productid`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ratings`
--

LOCK TABLES `ratings` WRITE;
/*!40000 ALTER TABLE `ratings` DISABLE KEYS */;
INSERT INTO `ratings` VALUES (6,2,'TItulo da bagaça','descrição lafdsafsdsfasf',_binary '','Ruim',1,_binary ''),(7,2,'asdfadfsfd','fasfdsdfdf',_binary '','Ruim',1,_binary ''),(8,2,'Guitarra perfeita','Adorei tem ótimo som.',_binary '','Ruim',1,_binary ''),(9,2,'Péssima','aaaaa horrivel',_binary '','Ruim',1,_binary ''),(10,30,'Guitarra linda','Adoro esse modelo, gostaria de ter 220 iguais.',_binary '','Ótimo',1,_binary ''),(11,12,'Complexo','Leitura muito cansativa, mas é interessante, é para quem gosta mesmo.',_binary '\0','Ruim',1,_binary ''),(12,10,'Otimo livro','lalalalalal ala lalla ',_binary '','Ótimo',8,_binary '');
/*!40000 ALTER TABLE `ratings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sellers`
--

DROP TABLE IF EXISTS `sellers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `sellers` (
  `SellerId` int(11) NOT NULL AUTO_INCREMENT,
  `Age` int(11) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `DateOfBirth` datetime DEFAULT NULL,
  `WebSite` varchar(255) DEFAULT NULL,
  `Company` varchar(150) DEFAULT NULL,
  `CNPJ` varchar(150) DEFAULT NULL,
  `BranchOfActivity` varchar(150) DEFAULT NULL,
  `FantasyName` varchar(150) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`SellerId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `sellers_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sellers`
--

LOCK TABLES `sellers` WRITE;
/*!40000 ALTER TABLE `sellers` DISABLE KEYS */;
INSERT INTO `sellers` VALUES (1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,2),(2,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,4),(3,NULL,'michaelcdr@hotmail.com',NULL,'https://www.akusticamusical.com.br/','AKUSTICA MUSICAL','43.142.324/3214-31','Venda de instrumentos musicais','AKUSTICA MUSICAL',5),(4,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,7);
/*!40000 ALTER TABLE `sellers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `similarproducts`
--

DROP TABLE IF EXISTS `similarproducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `similarproducts` (
  `SimilarProductId` int(11) NOT NULL AUTO_INCREMENT,
  `ParentProductId` int(11) NOT NULL,
  `ChildProductId` int(11) NOT NULL,
  PRIMARY KEY (`SimilarProductId`),
  KEY `ParentProductId` (`ParentProductId`),
  KEY `ChildProductId` (`ChildProductId`),
  CONSTRAINT `similarproducts_ibfk_1` FOREIGN KEY (`ParentProductId`) REFERENCES `products` (`productid`),
  CONSTRAINT `similarproducts_ibfk_2` FOREIGN KEY (`ChildProductId`) REFERENCES `products` (`productid`)
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `similarproducts`
--

LOCK TABLES `similarproducts` WRITE;
/*!40000 ALTER TABLE `similarproducts` DISABLE KEYS */;
INSERT INTO `similarproducts` VALUES (42,10,9),(43,10,8),(44,10,11),(45,10,12),(46,10,13);
/*!40000 ALTER TABLE `similarproducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `states`
--

DROP TABLE IF EXISTS `states`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `states` (
  `StateId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  `StateAbreviattion` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`StateId`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `states`
--

LOCK TABLES `states` WRITE;
/*!40000 ALTER TABLE `states` DISABLE KEYS */;
INSERT INTO `states` VALUES (1,'Rio Grande do Sul','RS'),(2,'Santa Catarina','SC');
/*!40000 ALTER TABLE `states` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `subcategories`
--

DROP TABLE IF EXISTS `subcategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `subcategories` (
  `SubCategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryId` int(11) DEFAULT NULL,
  `Title` varchar(255) NOT NULL,
  `Image` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`SubCategoryId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `subcategories_ibfk_1` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`categoryid`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `subcategories`
--

LOCK TABLES `subcategories` WRITE;
/*!40000 ALTER TABLE `subcategories` DISABLE KEYS */;
INSERT INTO `subcategories` VALUES (1,5,'Guitarras',NULL),(2,5,'Baixos',NULL),(3,5,'Violões',NULL),(4,6,'Tecnologia',NULL),(5,6,'Terror',NULL),(6,6,'Culinaria',NULL),(7,13,'Whey',NULL),(8,13,'Termogênicos',NULL),(9,13,'Barras de proteínas',NULL),(17,15,'sub categoria Teste 123',NULL);
/*!40000 ALTER TABLE `subcategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `users` (
  `UserId` int(11) NOT NULL AUTO_INCREMENT,
  `Login` varchar(100) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Role` varchar(45) NOT NULL,
  `Cpf` varchar(14) DEFAULT NULL,
  `LastName` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `Login` (`Login`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'michael','$argon2i$v=19$m=65536,t=4,p=1$YnJ3ZUVSQVA5SU1NaWQuLw$YeSBfMl2USToRCeIIZ9EJOotnCnHtQsd1MgWhITNrBA','michael','admin','',''),(2,'multisom','$argon2i$v=19$m=65536,t=4,p=1$dElNMWNHMWlkLnEzcEVuLw$vCOGlm+Qbcg6MRaCWjbjod27pTLjpEvVzr289lxTNg0','Multisom','vendedor','',''),(3,'tati','$argon2i$v=19$m=65536,t=4,p=1$R2VkcFFtUlY0TklhRTZUVA$Zsz6lol8gDdELrfVpEDhuPiM4CI+kSt1DfVKhV+HmXY','Tatiana','comum','','Pirondi'),(4,'robert','$argon2i$v=19$m=65536,t=4,p=1$M1dJcnVkZ2ZrLjJyNjQybw$RQOgsHSah5DTaxq1s+qre73fCuu3D7HLHjwdSKBwNvo','Robert','vendedor','','C. Martin'),(5,'akustica','$argon2i$v=19$m=65536,t=4,p=1$Y1VOTlVnV0FZTm8zeDRGUw$Q0miIuK6rVrEszp5y0kYsHwVVZf2Zeo7Qsioue7FHVo','AKUSTICA MUSICAL','vendedor','','AKUSTICA MUSICAL'),(7,'eric.evans','$argon2i$v=19$m=65536,t=4,p=1$VUFzcS5FMFZERlVYRDltNw$XBnTKOEmU5d18GaN9bfLKaHZWj5msC9tbONGRmYXE2A','Eric','vendedor','','Evans'),(8,'tatiana','$argon2i$v=19$m=65536,t=4,p=1$U0VQV25mdnhGTzEyUklXZA$P3/TU2iWhokjyfZEbB7z9nzKl4rOtBmJRxE0ousRwVM','tatiana','comum','','nao interessa');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-12-07 19:09:00
