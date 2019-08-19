﻿ALTER TABLE [ref_holding].[VALUATION] ADD [IndexBaseDate] [datetime]
ALTER TABLE [ref_holding].[VALUATION] ADD [IndexBaseValue] [float]
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201405061643191_IndexValuation_BaseValue', N'FGABusinessComponent.Migrations.Configuration',  0x1F8B0800000000000400ECBD07601C499625262F6DCA7B7F4AF54AD7E074A10880601324D8904010ECC188CDE692EC1D69472329AB2A81CA6556655D661640CCED9DBCF7DE7BEFBDF7DE7BEFBDF7BA3B9D4E27F7DFFF3F5C6664016CF6CE4ADAC99E2180AAC81F3F7E7C1F3F22FEC7BFF71F7CFC7BBC5B94E9655E3745B5FCECA3DDF1CE4769BE9C56B36279F1D947EBF67CFBE0A3DFE3E8374E1E9FCE16EFD29F34EDEEA11DBDB96C3EFB68DEB6AB4777EF36D379BEC89AF1A298D655539DB7E369B5B89BCDAABB7B3B3B0FEFEEEEDECD09C447042B4D1FBF5A2FDB6291F31FF4E749B59CE6AB769D955F54B3BC6CF473FAE635434D5F648BBC5965D3FCB38F9E7D7EFC64DD14CBBC694EAAC5AA5AE6CB76DCFBE4A3F4B82C32C2EE755E9E7F9466CB65D5662DE1FEE8AB267FDDD6D5F2E2F58A3EC8CA37D7AB9CDA9D676593EB981EB9E6B71DDECE1E8677D7BDF8B5C8F3911D381365B12AF377C08FC74F432F96D9724A381F374DDEE28B93ACCD2FAAFAFA84C8E6BF4CAFBFACAB555EB7D737BEFB518ABF88526D4D93FE51FA45F6EE79BEBC68E7C40E1FA5CF8A77F9CCFCAD44FA6A591087D02B6DBDA63FEFFA58DFF5D0DE3C9A938A98E036A86BC30D78EEFD6CE279F6FAECC5D98C78AA382FF27A33AA68CB03DA40D39F55644FD6754DF27B7DBC9C1D2F40B7CDF8FE6456AE2DB24FABF5A4EC741D990DEDC10E91046C6C3E94B1BF58976546A0ECD83E7030B760912E527DBAEFFF6C92FD655E4F8943B28BFC1549D507D2FC3DFA7D9A4FDAA7599B9D64E5745D8AE2D9D8F917D934A386D74FD7B5AAA9F7997BD2CE1082D9D77BFB0D69FC37D51759BBAE8BD64ED5EDDE250B7199BF7BEFD7BAC37D9D2F8AE3E5920CCD7BC2E90CFCEBC2B1C3F8BA00BE9B1717F3369F1D93C9266E7B5E9C7F53ACF4FB147939BB41CAE6D9F2227F5917D3FCF7DF7D1A28808E006C56019B217FF1E6670DF4EFF321A0DF839610CB5BD0939B3889101CBE2E7A83E0BF5BD54DFBB304FB45FEAE25DD537E18F8F7A4ECEB559D673790F6F3EA529BBD97789D2DDBBC5E55A449F3D9EBAB6CF575609C3C7DFDA45ACE9E644DD17C9DF77F2AAFAB9F040A450945F13520B08BF775D1FF720505773CFBE9750322DC08E13D26EF599DFFA2F5ED0CFAD97296BFB3ED7FAE5CD42765552D26797D715BFFEFC9931BBCBFBD9D9F4D7C5F9D9DDC16536AFA738AEAEBD3A75F3EBF2DB2DCF8E70CD593AF5E9FBDBC2DAADCF8E794B26F8AE9DBBCBE2DBED2FAE714E1936C55B41288DFAC18BCC61B10A65F7F3631C6AF5F9EB34F7133C65EE39F3B8CB9FB67D99423FFCDF83ECF9A36C0F67676E349FEB55E3B29AB26FF1AEF7DB9CA975FE3B5E7D5D5D778EBDBE474DFF8DA7BCC06BC18E19EDBC66D27659E7D9DF13E2DE8F7AFF1DEF1744AEC36637F8826F61B1AB70177FB61BB867D67007E66E852BCAF17FB616E6B84DAD93527ABD8F70B00DB91134DD69221FC226FE7D5EC16787F1D0247BBD988FBE0AB3F774AEB753E456054E4CD4FAC33B26614FDFF50A7934661399FC8736FEF46A991949B8F423F21F78D4DB64DE99E2DCFABE6060F2A5F16959F76799D97E763FBE9F3FC322F6FC18A11D5783DA98B995A6503FC4955415F7DED9145F0BADDE8B8FDA668E1DECF26BF3E2D1AEA72B286ECBCACCAE236814EFF9D9FAB60E769715990D338BB3DE67EFB1F0ED6A7AC053CA439D43D29B3A6215F771AB328BF577E1D7CE00DE4557E6E94DFACCBE7D166AFAB750D4BDA1189BBDD3E7ABAD52A5152239FEE4784CC5BA07ADD5675FE79BEA4041F05E02FB396F4F212307251813708A4C171783AF60E6E3521EFA908C249D8FD86ADC64DFDEDFD90FBBBF743EE6FFF87DCDFFD1F727F9FFE90FB7BF0B3DE1F6BA69B057F3310597BDC28CCB74235DEADEBF64576595C306162A3F8287D958B13DECC8B95FA0E11CDFBFB6BEB6775B578556190838D7E7FABA6AA9B5ABEC9EA0B400D0D84B30437DB878FD2E30959598A7E951E3F7B16C24CD8FF9B2DC40F8BA93ADD7AEE2A4DC67A41C8E28B0D78DCBBFFFE79A77EBF6679E5A9171DE0772C47DE1A67614F7AEBA2AA3B1168D0045F9866B7F3A7378B5EC7B5F9591044235E370BE280F7B3790000DA44F1C6377D44DDA73DCCBCAFBE162A3F99F1C2FB1009BF5D9533E2C001DA05DFC6891636795F14BFA2158FBABC26002FABBA3D27A7BA1AC6D536A1BF2A62BE0D933EDC343E8A0DED6343BAB5264617DF8CE2FD7F973665D2DCDCE1FF3B54F2D7B0F3378A69D7924724F86B31CC59D3ACF39AD926A5845AEEE9DC082F75BD3DA45A3A9A5A3FBB9D5AEE4E10237383BDFAF4FDCDD5ADA9E16B981E3D102D574BE2D7CD447999512AAA352B43C6A47870840DEDC2D1FB1289D617F26F20051633E1F55BABBFBF51C894A97AFBB301D7262C0D589AB6F2C6B7BE9BD3424348BC97EF99C2FC1A12FEDED62FEABBC70DE4D7E27566E7A6C07007197DB34B3F4885B3365FC4DD110BFAF71731F169D0FDAE67397B0DDED7FA8307875D3CFBEDEF2FBFE5331FBBFEB73DFC224D3EC898DF761EBEA6858F378B8656D1A6E2EAFFBFD96BF86119FC4EB77E0CE462A0AFAD538CA47CA8387515CAA0BC7D2D6675B2F5B3C79CFFEFE7B8AF37F53FB2F083F4A4357CA263E6AF65FB3D44BF7FCF3E7E9F222F9D920054FDE43DE100973E2CEFD3AF01EFF5AACEB33E40F3F17B42B4327AE3F2C537939D3DA9C8A72A48967A22F8C3D1DBB7C4EEEB09ED0D5E06BC876FC2C9E86AED0D7EC8D7D2DB67941179E79477D7171C52EBDD190314AC5536556D4959AD27377BE3FCE6CBBA98E681FE788F973FAFABA6F9BA2FBFE8E8C3F77975BD98E4F597E7AFF329529F05327096C9EFEDDD0A06A8FD7552A6F6E51B91BF3523D894D4073383210CB915ADA5492C36BB356E3CDC789CC211CC6674B28DF9846F26FF1DC6FB37BB1CB7985EC28970F5B9E3D69CF1ACCE7FD11A9637B01CF6D3AF979C016063D1A366FE6B817D962D8AF29A7CB86FDE1A0DC7A79DE44C444FF3687FFF6E43A7A963DFF77475B4D107696BA76B3AA88523FF9A4EF7FFBB7CE91F8ADC2A41AF7FF6F2754FAB45DEB4C534ECA1E381893A5B7FEDCCE997ED1C5E56740C31D27D0394C39F7591B759DD21DE8DCEE537D2FD93B2AA60662E021ADA4F3F60C25E9D9D0430E9EF0F80F6FAF4E997CF0378AFF1C907403CF9EAF5D9CB9073F0C907407C534CDFE6F5EBEBC5A42A03C0F2C58740AE33E42B6FCD990FDE5FA66FAD3BBD15BFFFFF3A125F37331D5D90F428769BF54EFBE17BAC79BA77DE37997B0B23EE800F1BF2A1363D633ED8F0830CFA2065FEFF68D1CF9697640A17D4F86725BDE5C0BFF29CE6AFB5C0D3817C7A7E9E4FDBE2F276B1DA372A83F1A5A2E1A6F175A30DED63ECBB69109E5278AF817C6D6572FB01F515D0D79249E313F6EC84CCC5FBA8EF1B85EB065784C605EBD98172DBA93AC956459B95AFB2A2194847698BDFDF8B2B680D3E5B82DEDE52C260ABFE1ADD70D39F05056F666A837E1F68D2E3AAA176EF2D1E94CA62CF60185D228AB6FAFD39F135730CD7C37BB06D8FF237BEF0BEF417E6DB4C77D326426FF96A98CEFAFD0749AA72DBFF1F6DE5AD2C4DE71D7234AD4DBD5D22F367C50E03580852A6099F7CADE8D605E99B156177BA7E769611060526A6447F36746E6F21F766F5FCB5A4ABA751BE19398B36FB79B2B4CB2AF91B163786D99339FC4A29787CF7B5648EDFBC3195F33533B33DBFF86D4098DBE9AE27C5EC6BBCF5C5D77A8B5FA175F9B647E7CE37EF49E56F72ED59E67AC34279BFC1D7E8A5B7DADEE9A4F7FDD7E8E3FFDDABE8FF1FB1465DD7F267DB1F1D70F46E7660BF966D52C7F31B3148FFEFB233DF149FBC2757736C7BF3383703F9C9CC5B8DFE59331732F7AFA7F37C633EF69BE98C87F4759CF12FAA6AF6B3B0A819EBE5EBA0F77AE9D6107EB690A33EBE0E6ACF8A763AFF59478E7BB9157A9B22E41B34AC06B99D864EABC6BEEF69D268A30FD29EB07EBDEC96C56F33D97EF6D6EB3B3DBDC8DFB5A486F29A92BA81ED361F7E889BF1ACA89BF665768D6CF1AD98A06B5DD72D7AFB8057CF9633E498AADABCFFA4A2D9CE96370A55B17CFB21EFF3BCBCA97E32AB8B0F0163DE07577E5D1898619AC4F26B5332AF5779BBCECAAF8BC017C5B258AC1767CB699D83130236730ECC4F50302DA6FBBDB96C4961780104CFAB2680DEFD6A23E45B0BF6E92F5A339EFFAF12ED5B63EF16709EAD97B3AF398AA74543684FD6ACAF693160EA86C38E7FEFEBAF151E3F2D2E0B72E866D11EEC57807E33F45BD3C7E40A4EDF4DE7D9F2A2BB5CFD35DDDF68B39F27F918644C0D558349341F7E2DE6F889754523FAE6E19A79F797376F97BF8017CB54FF59C9E80C31B0B736673C2652CBBAA419E27BBA9CA5EAEDD826CE1172F4C3B784EFBA6C8B15491E75FAD947DFEA8D3E0ACD2E983868BAB81A82DB0D5123705F2E9FE665DEE6E9F1146341C6B59966B33ED5890EB3F0139228F255883A5979429E615B67C5B2ED8B5F41A6609595834877DEB8B564C71B6A84D95303772D22DD6F9EE62B5276348EC149BA4DC736C6FC40349F1A6C3AD4BF89D88FEF7A1CB9995119D56F57E50C9EF76686ED378D31AEDFEA7D183802FD87CFC8EF41B8936AB1AA963409BFFFCB0C733148B66EC318D16C9BF7A1580F70845EDCA629DA9E2ED9198F77BBCAEDD663B75AF6F797DF68D17D08C97ED3D8F86DABF7197F04F48751E0EBAAB161446E25FE1FAA27A28D637ECD7B69BEE189BB4DF744BC964897D7B746F8BD4667C10F0DF3E9CFBEE6A4582C7FF7FB8B17778EA80CEC3BC4ABB1C63141E076EF230451C01131F0D64C438C7FD674E9D795A64D23BA0D6F7CA0C5DF3453B7EBDE7FF3E7D6AEBFACEAF69C02B7CAFBFE36467EF8BD5BDBE4A876BD7D37438E45EC959BBBFE46E8663FFC1AB4B31FC6E8E700DF3890F7EEEEE78C8E0E855B6AC8A117624318A4D8264D39D8C1FFA7B5E54DA3BA9DCAFA208D79D3CCDD0E85FF5768CD936C55B459F9FB7BF3DF349414ED8EC2F7C2075F893AFAD2FA7DF8764307C39C7B7DB3307F5D8EBB199FDB4DF82D5DBD6F2088BF798A6E8388A1EC3780F9D39F7D4EB6EB67B754BF03ED633C3CC4629B987808FCFFA775EF0D83BA158F7C1867DF306BB7C3E0C335EF37C7AF34EB34F8297240F8E92D26DFC059832F6EE060F7CED760E5E10E7F4EB4F26DB1BA1D4B7CB0867B5F0EBE71FA6E83CEFFB734342D6E60B858E3B899CB638DA3CB06DCEE7DD8390AF93D58F8E75E0D6F1AC1ADB8E0C3D9E5D6ECBE691E6F8302076FDF00C24F7FF6F9DB0E4F79F2067D6ADA7DE31E87828DB0745C58BE719DDC45E15633F7616C3640D3DBF42C6F7C0DAEF966B88619FCA4CC9AC67933FCD9E6AC4BEC8DC1E447D8F87D986AB8AFDBE6C57EEED5E58D63B80D977C032AE8D6CC7CE304DF068FFFD72ACE537AA7BDB60B1A8AC0B3CF8FF151FEAECFF768FE3A6F87E5A5F928955637F17D8FCD23A00781DDF83A262FF6363EBFF165BBB61983E0AD96DE00C62E62C5C0788B8E3780F143D218A4E190F536248EE56207C91E4FDCDE444D49804469691252B72681FAE99BE960E3AA9B9884AD4D944DD438DF34B4754D6237BD3E7D379D67CB8B28C775DBF4807AB40CF9574D4FEA35F0B83866980273E11A595B64F1B702D2D331D1F78D7DF1DE37F2D93513E1686E315206F4EDAA9CC155181C71A4D530E6FDC6310AF852BE810C11603F9BE4B058FDFE2F33A8F418317A6D86B1EF36FD0042F44045C8300CEA6B90C2AAC8DF5F7ECB673162445A0D8FA1DF3846105F716F204804D8CF3649A2CBFE11A244DB0D8F64D3B2F3EDC67233B808693A96ED83A93368A36ED02BB75DA9EEEB855BAC55DFA824DE0B6A6452065FF9D925A9B70CFA3E641D58C8BE2511FA4BD9EFCF9DB781FEC326F3E0827584B6B75BDC0E867CE3F2F6FB53F1C605ED9F45390F96D404AE59288D59CB5B2EAB86C6EEC655BB8066D6C5DD643F6F5CBABC59577C0D62D96CCCCD9C35D474784C036F0CCACF660A0D41FB21B0D44D2B011BC875CB55A3D8486F5C78880FD905413713F3C635999BE7E86B9033BABC100B6462ED86C7B47121C21B870DEC36853731583F9B34E9A445370A602C6B1D17954EDEFA46C43742890C7F80945F63FC8C4F34B93AE440DC9C89ED1BF68DB9D82E757AA9AB9BDC868DD9D71B69BF8168C8AFE26D9B8AB3DF3DBEFB7A3ACF17997EF0F82E3599E6AB769D955F54B3BC6CCC175F64AB15A6CABDA99FA4AF57D9140668FBF547E9BB45B96C3EFB68DEB6AB4777EF360CBA192F8A695D35D5793B9E568BBBD9ACBABBB7B3F3F0EEEEEEDD85C0B83B6D7CCA3FEE606B7B6AAB3ABBC83BDFC2A998E5CF8ABA699F666D36C91A9A8B93D9A2D7ECE6C4A3E96843FEB13F8578171919F3327EB799CE27EB863A6F1AEBF78CFB9F44FA89E47D15FC33A2C082DE626228298658AE0F85E0BC9E6665569BBCB097F33EA9CAF562E9FEEE72EBF0DB46007C08E6B3DB430971DF0DC1F5BEFCBA70F736C1DDFBFA70EF6D827BEFEBC3DDDF0477FFEBC3BDBF09EEFDAF0FF7D34D703FFDFA701F6C82FBE07DE0B2A07419DE7E787B38662125909B81C51568D68E00771589AA684F9374B47B574BDD4A874922F67D94D65983DFBF3CDFBAA5F6420F77BE86BEC27B3F2C05F5FFCD29FF66ACCB198581791DA7F6CFD52401BB327FD715F86ABD6CEB6B59687EAF31EA9BF00362D80E6A18D361A059CC871EE60ECEDD0EEAB7A70D4F037EEFD0C8FB7C8056CB59012597220F6CDFE09909203D2D9A695D2C8A654693F8FF5E7EFC7F1B2776A8FB7340D7AFA9DABD54DACFC234B176D485A9AF315DC7AF5F9FBEF9FDBFFDE5F3A7672F3EFF5998B7F752CFEF03834288EEFCF327EFA1C76401AB9B107BEF1990D10890BC7E1FBD8637591706E350B486E9711BDD161FF1338A008F17509C5F639C6609FB78395318EF315496DA709C3E32B1710EDB1F45E403C6306C81066D90ED343E86DF9F1A0C8EE33633769B36717A7C91D56FF35629FCB549F24D4D6B80CD0F6B5EBFE979F506F17338B14FAAEAEDFF5BA6D5C3E5FFAB936A87F04399D228623F412B3DB0BD2162EED3F7B05EDFCD8B8BF9D7D1E32F734AF850B2ED2247D6FB03D9C260F1F50CD5877A64DF8C0B75B69CE5EFBE86EF74F6E2E9E9EFFDFF5B9F290A01FF8610E493DB4308DDAD3E4DBADFBE07644CE3B3E21D4D597FA0BD2FDF43D2E4DD3AFF45EBAFA9C8ECBBEF1BF8767BEE8FC87DF7F52470F3A07F5695F7FBA8EE0E365F7BACD12E9F658BA2BCFEBDF29E6F673FFEE663C96F4673BDACEAF6BC2A8BEA6B68AF975FBE7AF3ECCBE7675FFE4883FDB035D8CF32F3BC6F66D8FEF675D2C3F6E5FF7FB2D1874ED5D7CC19C1D3FA1A0B99EF3BF3B69BAF33F3F6E51FD6CCDF7ED606ADDA8F1220379AE5F733CC3F4A80FC2801F2B337B13F4A807CA393FAC34D80C4C9F1349FB4A4B6B393AC9CAECBAF9B738F41798FE9FD229B66F4DAF5D375AD1804568580FFFEEEAB0132C5015733AC02CC36007E9D2F1B5AC9BA1CCCF70CC17E532CF237D51759BBAE7B192486FCE6CD17EF079116D52EF3777160DE77EF03B24BD9D7F9A2385E2ED759D9EFA33381BFFFA677DF0B87CE24BC170E1BDE7D1F1C2CF9DEA7F3E84BEFD3AB64E3F2D9F1654E5E5BFEBC38EF3A4C915EA32F7DED083B2EF5BF4F9197B3AF21E7F2DE8DEAD6F404C2CFB3E545FEB22EA6F9EFBFFBF46B74F9F216E9D1F7301B3C82DF3FC4EAF7DF6C476E41E6616BE375F4C59BFF578E9FD0FA2111E0F7F97F27017E9F6F8200B76A336C82BFAE40BA77DF6752F80D67BB98AEEFDFF7373C316CE67E9F375FFCFE82CED79F8A9B86FDDDAA6EDAFFF78C5918B28BDB8773E44D647891BF6BC9EE94FF2FA5848FDE8713E3566D86C5F3F5AACEB3AF2B9FFAF27BF80D9F5797A6C72E8DE4F3DFDF6BF13E0EC9D9B2CDEB55459E463E7B7D95AD6EE865A8F9FB7479F2F4F5936A397B923545734377B1A6EFD3D54FE575F593C0B628E1B46DEE2CDEF87DBA3B6E9ABCBD05157BEDDEA7932F57700A8F673FBD6E30099B7B8A37FEDABEE3A0EC8A2B5EAD490384B8F4BEBC7D36919CED3623E9A9BB49C9E08BDBC31BCA4BBF1F14DB793FD9D9F9AA0FF34373D5E937B226C54B751B72C3FF1FCF2B0FA8391AF3538AE99BAAEE60127CF39E10D95D8CD8AEDE97EF09F7F3BA6A9A21B8FE97EF09F7854D7BF6A0BAAFDE17E67A31C9EB2FCF5FE753F88E45DEC4A0471ABD673F6404F23E2374BEFA1A308708E27D17854AD23E2B200529B7A2B75E3C3DFDBD07B41F84F4FFBD1AC1AE52FF3CD30A862B5F6675DBE1DACE57B7E380976F9EFD90E7FF6BAE5E3A313C9BBDDF02E62D39CAEF20F796DBBF065FDD1ED4CF0E9BE1DF0E6FF027032C110B187408D79D01BC3F59C55D112079FD3E0104DEE4558A9EFBF301EEE04078542DF2A62DA6E1688DFFF7DE633EA9D6CBB6DEB0C432E4A7F16B5D0F4D3FFCDA438EBBE4ED1CCE6838B97EBFD106B76741FC5917799BD51D168A79DC37367E0FC67D525615F4E0C5D79837FBEED763D8274FFAECEAA1F3B527303ED05767275F6388F4D6D71B1CBDD81FDDAB7C4DC1F590B1F9FA637B7DFAF4CBE75F6374FCDED71B9F766947E73EFA86C776F2D5EBB3975F636CFCDED71B1BBFDA9F3DC5E41B1EDF9B62FA36AF5F5F2F2655F9358629AF7FBD71CABBFD81CAE71F30D2786F7536232F6093121D68F2FF1AA78AB33AD695A6BFAA69C138FEAC785883BD7D0D17EB3D607DB08F15E7F3335A576D5A2077BC808DFE1ABC7EB2AEEB7C39BD3E5ECE14C67B307B3408ECA2141BCFB06652743E6024C32ECFA06EB29D6E1AC9EF4FCD0647731B09BE4D9B9BE6F9677191E16B4CF1F00AD3AD061BEDF0F4FC3C9FB6C565246BD1F9EAFF5D2AEC67455D690874FD75B5D3CF82267AAF24F17BC1082C541F9FEEB7B7874C6CCA16B0E338DA4F6F97A6B07311264F8A665A178B6299B5F14CE887F2E437C34858DCF81A4CF4F4F4C99BFF2FF1D0B36C5194D7BF57DE9924EFE3F7B0AE5839E5953BD2765F43E79A574FB272BA2E071D836153E8DE7B56E7BF68FD35CDA27DF7FDED222774BDBE836C8E479CDF1FA8A2DD8798C7413AFC2C9ABCF7307AC17887EDDE078EF56976CDC9165EBAFD1A83B63C479FAC5BE69D2FF2765ECDBECEE40F80DA409700FDDF7F0384AF4FBA5BB589EB86A26EDA97D935145DDFB7E87F7B7BADF392463729230E4BF0C57BC323F183B9EB2EBDF5BFBD3DE4D7C5F2ED06D091AF6F0FFB59F12E9FBDA1C590BAD8D0C570ABDBF7645E86180E7433D0E4F67D80AF49AD95F189ED7F7B7BC8A4985679BBCECAA1F98D7CFF1E56EB8B62592CD68BB3E5B4CEC1CC5F438BB8B5839F5867EC0CBC97E1FA7F87C2EED28195F6EFBF6169F276BA650883AF9645DB43C09B84F78A827FB6A3F90F21A306C31B0939ECD3FCEC86F743E38A07F8434333ADED2F1B86793B8EB945AB5BB58913F519051F4B4A3D91C238AFBE8ED7D001F03E9CFA3A5F16954445EFDDAD7DF7797E9997EFEFA184EF77CD8C3FA4DFDF3675BFE94BDFBC26F8F6F5A42E6627D9AA68B3CD58755A7663C7DB2172537C99DE1CE5FD2855E0DBAE6F2055C0D1F6FF17D304A7BF681DB5F637B2D0E94F7C75F6E6F7F9FF120F3DFB1AA9821FC9DAFFFB644D59F6FF8BD2E612E9CFD6CBD9D790BA675FBD78FAFF525E8ABB0B34316D5D4CD698C097B46AF6B53CC13E9061DF213E96181A1DFEE97CFFFBC75EF97A067B03712E0B12A7D90710C607F0FE4409BB0F09E27FF7FB779B7E3D42FC489BFEBF4F9B7654D2FFABB4EA5983DFBF3CDFBAA5383073DCF91A6AF5FF735CE5C515A4A5D61814BE180C3EC246B7EFE78BACE525B07E422CFCE63D2C82C58A897E42AF5F54F5D7517E2120BC6E80BDAF221C863440D000F5DF7FD3EB3F3B8AF26769595B03D39F95856D13F4BEBF700EBEF9B3239E7D567FDF7C6F3F33289FBC8790FC6CE7036F9D0DBC4DF2EFE726F5F73E89BFF7CDF3DD2CA1B76C13A70CE8F175A82282C00AE63D359CF76A48197CF2FB07DF7E3D8D35D4B1AE2C5C7765D2FFFCF69235643AE3507E8EB4A85B4C79591753FAFC67459FF67AF91A9AF51630FE5FA063E332048CBF96107DC30A52F1F8FFAA6664F4A10F7F8E5421F7FF35F5A104070CE17DF5A1F76A8C1E3F1B9A9021BFAED675BCCBD7D5BA1E62A438C0E3E6EDE000E8BBF701F5A4980D82A2EFDE07D4171B407D31086A03733CCBA6EDD76490F0FDF7608FE759D30E8DC2C0FBFDBD4603A213A7757E0BD85EA3F7817D52564D7E2370BFD5FB40FF72952F6F04EE357A1FD8CFABAB1B41BB36EF03F9DBC5C5FC46D05EA3AF2DF5711EFE7D8ABC9C7D0DDE95F7DECBAACCB3E585CCEBEFBFFBF46B74F932270DB46CB38BFC15ACF17B59978865E411FCFE2156370412B720F3AD86FFC59BFF578E9FD0FA2111E0F7F97F27017E9F6F8200B76A1327119669199393AC9CAE4B4EBA7E0D3A45C1BC874A3A29F32CA64901F7F7972FDF4FC73D2DE8F72180F2E5FB013C9E4EEB753E3B5BB6794DA62802F5F8EC1B5795804B7140F6E193D385F21E23FF229B66F4DAF5D375AD18F4096ABF7A1F927E51CD28D19FCF36007E9D2F1B0AC72E7979F37D60BF2916F99BCAA4642390DFBCF9E2FD20D23AC165FE2E0ECCFBEE7D407629FB3A5F14C7CBE59A527BBD3E3A13F8FB6F7AF7BD70E84CC27BE1B0E1DDF7C1C192EF7D3A8FBEF43EBD7E372717A7CD67C79739A503F2E7C5794457747B8DBEF4B320F55FD74972EFBE8FA1E4379CB8B0AD7BFFBEBF6163C992F5FBBCF9E2F71774BEBE79BC69D8DFADEAA6FD7FCF98C549E8E2F6E15EC24D647891BFA3159CB2FC7F29257CF43E9C18B76A332C9EAF57759E7D5DF9D497DF43557D5E5D9A1EBB3492CF7F7FAFC5FBE8407667561529B77CF6FA2A5BDDD0CB50F3F7E9F2E4E9EB27B4E4FE246B8AE686EE624DDFA7AB9FCAEBEA27816D51C24E6CEE2CDEF87DBAE3C5CE5B50B1D7EE7D3AF972053B743CFBE9758349D8DC53BCF1D7365751847E1EAE67900AFAD95AC410D05F63E562E8C541427789FCB3384D1B05A60BC67E787B3811F3B1C140C46108FD5E4FE7F9A2032AFCE6F6101985FEBA8EF7F1ED617D5155B34EE0A11FBD278C3E3EDEC7B787F51AF17820ED4301FAE0FB7D4CEC87068E0F270EE759D14EE72114FDE83D61F4B1F13EFE7F8DE2312B56A7EFA69C43FA595141DD4EBE8632BA19C4CF8E5AFA065651BF5A16EDCFEA6AE48057145D8B0C91F9DA763B3ED29F5857E414FCBF66AC5D74BEF668A37D1A4E7CD597F3DFFB7D380CDA5B721D9175CBDE973F7CC541D6B39A169A8DE9680FB6ACDFAECA197D244EE8D798F41890AEF81FBF7E7DFAE6F7FFF697CF9F9EBDF83CA2A266313F2080F9FBBFC9EA0B02DDA7DFADB4C6061FE2F6FE8C00A12FFA60300B76105F6B7CCA21B745EF565AF1F6438B8FEA43B5EA46AA50BA6E56802DD3B3E6C5BA2C3FFBE83C2B9B0EDCC1797B9FD723637B7C372A17B7171DCBFFBFFFCB8CF4D3D7119C3E889ED5342D6E21325D681F283002E443D94AA17C20730994AFC162B7A0D28FC42E2637C3B3FF5EEFC755E57B40888DEF83459795C2CBAA6ECFABB2A8BC961F66023741ECD9C3A1176E21E83774F681722F303E94AF15CA40FCFF9E327BD380BF7911FE70011A26E37BBD1F25E0CFA200D80FBF5121F0A0FE7004C17EF881C2E0E07CA8407890867397B754FA0ED6D750FF5F8F8CFF6F14B1CD93F3DE308674D57BC2F97A16CB0471D4599B915CD5DD26364AD44FECDF8DF90092454B6D5F50E85D9A0F7982284B99F1C434AB6C9AC3059AE5CF0A5A44C44AF6246B7269F2514AB37159CCF29AF26ED7B43AB018A3C1F8F52F2A4FCA82DD53D3E08B6C49ABDC4DFBA67A9B2F3FFB686F67E7E0A3F4B82CB20649FEF2FCA3F4DDA25CD21FF3B65D3DBA7BB7E10E9AF1A298D655539DB7E369B5B89BCDAABBF4EAC3BBBBBB77F3D9E26ED3CC4A9FC3BCCC95CFA92765D634C579318DA889C7BF57DEE325C363AFF2738F3D7BD3DC6F160FD91FDFEDF6615FF5C003E9CF3E9A14178575EB3FCF695AB14CF6326B69CD6C8986398FF0A3147C954DCADCF256A7CF4E0F0633E9657999D594CAA83F4ABFC8DE3DCF9717EDFCB38F76F70EDE1B6C48D9DD2EFCAD45F6EE8E0FB4ADFB09F5CD30F77E1660DEFB5980B9FFB300F3FECF02CC4F7F16603EF84660B2A8F625E13D39D228654568DAE7F21B40FAC9B38DAAE5555576CCDBD7D525FF6F5010FF2FA17E84D5AAF5B2AD35B51C831980BC0DA39D35CD3AAFF1FB467DF8E9CEFB027E5A34D3BA5814CB8C26E70375EDADD97020E1F3757931DE2C1E95F71BC65D999F7BEE96017C835C2903158033FABD2DC04DEF09C4A54C8684EE365CE7274E6E37C2DB43BD69987D48B7E65BBBCEF1B3C8B7FF6F65C76F847B9E916B7EBC807634A0CECB2A7B6FF671607E7F5A371B66A1FDF705FC4556BFCD5B4CB31DEBD742D083F3A11876203FA9AAB71F8E9F85F20D63F7349FB4BFFFD3752D42F2211832A4D7F9B2A1A0F492F9F54381BD79F3C58703A1C8F5327FF74DE083A8F3242BA7EB52B2105F64D38C7EBF36D47B9D2F8AE3E5729D95DF7C57D58CFCE07CF643E8CAD2EB67B18FEFE6C5C59C74DCF1658ED4C0730ADA3FA88FDFA7C8CBD9EF7FC2CBE62FEB629AFFFEBB4F7FFF0F97B93ED82FDEFCECC0FD7DBE11B8CCEFBFCF9B2F7EFF579EE6FFDA90044BFEF74DF5DD8AF23200FB0DA119007F91BF6B893DCA6F10FEEB559D67B3DFFFF3EA527EFBA6009E2DC928AFAA1216FAF555B6FA66A19F3C7DFD84B27A4FB2A668BE59C83F95D7D54F02EDA284687FA3B039A8FBE689F1E50ABAE278F6D3EB06C4FE70D8D6178CA7A9BE563AC1E6453FD4BF7EBF98F636102D6ADF881F78B69CE5EF9E92616F5CD0F9B5668101B1D6FB704967589F53EAB6F98660BDF8265C4981B45E4CF2FACBF3D7F9745D9347943706E6D7610E4024AD90BF7FA4B401D8878FD40CF26556B7767C24A7E5FB02B2A209105DC1DC94D3E803BF7568E8664622AC6F3673FFFF860010FF6E24E6BDFBEF9D7DFAA63300DF7CE2EDCB760E751C4CE96622BC6F0FF8B32EF236ABAFC37E6EB1FEF1FEBD3D29AB0A6276B18146EF3D8DAFF235319795D96F02E4EBD3A75F3EFF26019E7CF5FAECE53709F04D317D9B6F089EBF06C43A9B15CB8BF7E0B60737F4716BF535B83AFEFF1FFD75469168D32EE8ED6F2003D505F60DE7511CF80F0EBE4ECFCFF329E550BE116BCF6CF2FB7FA877AA50BE711FD5F2EF0763E841FA59C4F2FD67E4FDE4F99B91DD78B3C8EACAFF2B84FC3D1C8A5B017C462B634BD285E5D9B2A1F98048FE6C38425F642DDCC7EBF767890D2833139C10948BAABEFEFDC38FD185F9CA74D8A7D7FB761D9AAE0F5E9A25E5572C2F3E54987F6E9638B355D152E8F28D88E0CD92A5E1C7F587D2EA675F42BF090EFF6A597C98F956A3FDE131ABB3FEB4F438BDB6BF7C83BE0000FDFECA4EF8BDCBC15F2BD3F4A166EDD672E0E2626469489ABF19898836FBFFF5D22927B93E88576571E09B755405E637C69602EE1BCCA80AC0E3E6ED3740B92785E5940F80F2C537018516C0D982FFFECFB3A6FD8638C3C27C927FF3304FCAAAC9BF69A05FAEF2E5370DF37975F54D83FC362D4F7E7D9806E68F56254597F22A7C99674B9BCCF9FA709E16F4C13700E7F8ECC36198C5F80F87F43A5F3664712FD9547D28B0376FBEF87020B4787499BFFB26F0E9AEFD7F914D33FAFDDA50EF75BE288E97CB35160DBEE9AEAA194536F9EC87D095A5D7CF621FDFCD492D917B737C496ECE45FEBC38FF0604F3F779F3C5EFFFC119ABDE8AFE77ABBAE144D837A43F02E02FF277142397E537085F979C3FAF2EBF8175660FE0D9921CD115D6DD6905FB1B5F213F79FAFA49B59CD15A5ED17CB3907F2AAFAB9F04DA45099EFE46617332E39B27C6972B08C9F1ECA7D70D88FDE1B0DF2740DF04E78716B849EEE59B89D6FEDF1084BD1FE16E059279EF83935C81CEF9A06047A6ECF5749E2FBE19808CDA3791B0F9A2AA662E27F2212831A46F02A5D7CE81FC208408CE3781CEB3A29DCEBF118418D2FBA3746BC560525CA7EFA6ECDD7F332A22DAECFFD7091D24317F16D2853FB1AE688C3F0B809FFDDE1F6400A14BC46FFEBAD99D5B73E8F1EBD7A76F7EFF6F7FF9FCE9D98BCF7F16D933B608F6B3CAC7EFC961DFF862D837C2F72F33E2CCF67D70BB0D7B7D33B658A0BC076AB7824A99A05C160A3E48821C986F3889FB4556BFCDDB0F8F7D3C38DF30864FAAEAED87E367A17CC3D8FDC43A533B2220295828DF178684E3EF31BC5B6BC3A7A74FDEFC9095E0FF1775DBB36C5194D784F7D7B04C7D68C82A70B84EB9F4DF1F69976775FE8B0CE40F5F670FC07F70C22580F634BB3E818EE13CC0EF6F3E3EA916AB75CBF6FB8BBC9D572E90FA102A3D2B28B1F332BB5E9041787F8F35625A08456AFC0D823A5BCE8A6946EEA363D7F726EFEB62F9F69B82F5AC789753D2EA27B3BAF8A6401A5860A36F029E49A87D63334139AFBCA514E83781DC17C5B258AC1767CB699D83EB7EFF6F28ED47702D4C03A8A3D66E0926444FEDFD37826014B20914EC2FDFA05D7C562CB3E5B4C0CC9D570DD6258AAA26E3E47E7B9E5FE6E5B05ABCF7813D7EFB7A5217B3936C55B42E837E23D7DCDAB29EFEC457676F7E9F1FD9D61B017E886DBDF56C9CBD787AFA7BFF902723DAF047D19E6F0D32BC14CE7A80D8BDFB3BEF2BE592B7398731207FE4C309B79CE5EFC8B052EED41FB20CF7659D4F8B86BAF9ECA307EF8D270326C76FED2BD61BBDBF10EE10DC9F0D7DFDC310D3675FBD78FA4396D2FF4F0A5FD1B475315983C35F56654116BAFFD137C7524F8BCB82A46A66BBF2FFFCBADDDC9A275E7EF9EACDB32F9F9F7DF943668C1FA9EFFFCFA9EF219EA2CC5D458E1F206A37AFAA3297E5E10E579D2E6729BED4749FE9FE755E9E8FF5932FD6655BAC88F7A927B079976BBE5C3ECDCBBCCDD3E369CBB6E1246BA6D92CC237D4D950EFF837E85C3E08FBFE560F24716A8E0C2A79B927D59254424654EBB375417EF02A2B8391765ADD5A9CDE47A0EEDAAEBBDF3CCD57A45208737FFCB7E9C9E6766F8D575427DC7D6A10E8CCD14D147D7CD7E3AECD4C873C49B5447CF592D3DC83AC671B061CE07D1AB2C1CE78BCDBE384083440B91DC46F88B11CFC5BCDCDFB4CE22D9BC6F4F57BF16148BCDB7429737BEBC168F3DB0F495E181AD8D39F7D36B68B75BFBFFC96CF7E488C6C3B0EA0799FFE8889E59BA71D267624BA4D77449A960893D7EF35965B37B6E07F0E59988DC6CBAA6ECFC987ADBCEFBF199FE0063E1EECBD0F39DEEA6785CFFFDFE8050CD3E036DDF3DBBFFFADB1D5E63F971EC2305FDA0F0779D3C5493E17799FFEFF80470762416DFB0DF369B4E907EBE20F6369FBE2EDD9DA7BE57DD4B47BEDE7504F6B769ED604A66B5A1228F2E6AC6968417DDA890DBE7115DD591610B7C57CF6B3C2DAFF6F54BF66C4B7E94CA7E8FABDB0FB3953B406DBDF3FCC370C7295C780E10B3E830C370A39E6672965704BCEFF86F87570B0B79AFBE8BCDF9A2FDF47567CDC7EAED98D8845639CD25AC2EF8F9FF9CCB0E1ADB5D9D7D366BDEE07D8D67EFBB3C231EF336BEFA1433E8893FA63BF4DB7FFDFD275AFE80FE2B85788D4BF2986FB595261826A98F7D48F7EDEB0A40EF8367D31FAEF85D9CF19135A831B1BDED7E1801B54DE2DF9F81BE2A3F798B31F9AE9139C7ECE269C113D29B3A6B1E6F79B49AEFC2CA99E08BE7D5CBADFFFAC30D3EDA7F8EC7D44FFC3D9AE33FADB74FCFF210DC5A87EBB2A673096987807F0F77F56578BDFDF2DDFBCA97EFFE3D7AF4FDFFCFEDFFEF2F9D3B3179F7F53F9EF8D0A2DE830E0CCF09B1064544D7E2DB6FCFF411E7CC3A471FB9FDB613CFD21F3F8FFABB5F1D7E3F6FF3F6BE0F7E6DDF7D3BDD27A00CBA73FFBACF9349FB451B5CB8841E53E3D7DF2E6C3D97558C3327C1F827C1002F8E6F4E9FF1B99AC4F626EF6B380D2D39F7D8E3AFD456B041C1B79EAF427BE3A7BF3FBFC6C7295F6E0C3301FFD7CE2AC18A1B9E1CF02524F7FF679EB8CE0BFBBD94B3C7BF1F4F4F7FE617887D2910F463FF9D9E2B1FF1F788391C9E1763FB7E83FFD61316F981AEFF871FF1F5BFAB825FB7F43EEE1CFE5D2C7FB30AD3F113F6761F5D9F2326FDA05017BB65ECE365BE3675FBD78FAB3698B19BE0F413EF8D9D291FF6FB4C37D1273B39F05949EFEECF3D6CBAA6ECFABB2A86EB6C42FBF7CF5E6D997CFCFBEFC615863D7990FCAFBF4678BE3FE7F609507268ADBFEDC0EE1E90F93A1FFFF649DDF531CFE7F60A5DF97897F0E2CF529BDD35ED33B2DBD91D78ACA4935CB9F1575D392106493ACC97B8C87B75EE7ADB66713D75D15914637AD9DBC9ECEF345F6D947757EFEFB3766593A7D934D5CB6EBE439FD3C7B767672FCE6ECCB9EF8F69001ED23BDCBC77E7745D3ACF3DA76F6EACBE7A73702F70C41AF07EF3BBF9BB9245A6D3F275F7EF1F2CB17A72FDEDCD8D94F66E57A8898DE779B3AFBC9E3E75FDD8E6CC3DAA1D7F770531F9569B558B006124CCE9ED2906F3F8BCC2C56117ADC3BC458F1B6B7602F4F03DD06A7A1FE6FD3D78D3D9C64ABA2CDCA1873996F36F6F2FAF4E4AB57676FCE4E5FFFFE67AF5F7F75FCE2E4669676B3498A6DCACCB361C66D9B8D78BC7C75768B9ECD927E5F56F50BBF8F5A3F33D24A6C84CCFF4D045DD7A417A7D7A7EFA6F36C7911530CFD261B87F6ECF7FEFDA9EF9B07D759A1E8334DF8FD2621EE34BDA163C957F7FA938F370E4D9ADC00DEA42D7B1D982F3676611ADDD089C6EDBD3EF4F34DC4D226377420215F0FBE7CBC79FEB9C90DE03D9DD2EBC3FB6ED33886D592A7E0A44763F5742D2FF51A78C62FB6D2173868817A23C8F6939E1FE2DEF18D2DBF221F74DDC210E55B0CC7456D2F333836B141F5DA0CA3D933DB8CABF7E98621DA56E8F476EF7F8D015B7BFEFBCB6FF92C36E448AB5B207E3BA483777BAE07BFEB7DFAC1031E34DAC35C7CD32BDF2C6B6FF02ABA70E2AD7E1649643F7C3F32B9D78687DD535D3CCC416DF4FF3292A99BF4FB7B2E2A79F9D9729A4735C870EB6F96973A7E9D08A2F9EC8307ADE85F77D205B1110F351D46FDA6B880C732DCE8266EB999905F9F20CE5DFDFDF1339F99D16FA0CCF03B1F3C9001B276BCEE0E3DEDB73EACAF451471AA7FFF57B01A9B08116DF7CD0E3E74FCF925F3D1070FD372B881B84108B4C93780E8D721C9D7181CC30953283798CB68E30F46BDFF4E3CFBE32074BFFF6648F16D719A6F4EFC7742A801627D4D701BF4FE07B860D1005208BA311EFC40526E66A7B0D537CC473FB4013FCD27EDE635480985FB44B8E59BDF2C61FC889E5F8906EAEF4F86D35FB48616DC381C13B0F749F11E6F7FB3E43028792F998F3E982467945D7F77B3FC6B92A14F94F77AFF67476F04B9137E2F9E12F9BAC4E9AE490D5261E3E25580F2CF9E8FF9B3498CCBBC691784C9B3F572B6590C246714A3D37BC2F86605C94F82F12BD1DCD6FB93E66B2DC847C8F3B5E0FCEC08D57B46C51F44B45B08D82D57887F4842F6CD11070BC800601724ED778FEF4AB2543FA03FDBAACE2EF22F68A9B26CF8535A065DD3DB8B5CFE7A9A37C58503F198602E735EBF76404D9BB3E5796556643B189926E66B9DAE2FF2369BD1EAE8714D44CAA62D7D3DCDC9C34568803419ECD46292CFCE965FAEDBD5BA85342E26E5B54F0CACE76EEAFFF1DD1ECE8FBF5CE1AFE69B1802A159D010F22F974FD64539B3783FCBCAA693C31D028185E2CF73FA5CE692C4B3CD2FAE2DA4172458B703A4E47B6AD6B7DFE48B5549C09A2F97AFB3CB7C18B79B691852ECF1D322BBA8B345A330DCFBF427B1DF6CF1EEE8FF090000FFFFF5938E6350D70100 , N'6.0.1-21010')
